using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AnkrSDK.WalletConnect.VersionShared;
using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.WalletConnect.VersionShared.Models;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum.Types;
using AnkrSDK.WalletConnect.VersionShared.Utils;
using AnkrSDK.WalletConnect2.Events;
using AnkrSDK.WalletConnect2.RpcRequests;
using AnkrSDK.WalletConnect2.RpcResponses;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WalletConnectSharp.Common.Model.Errors;
using WalletConnectSharp.Network.Models;
using WalletConnectSharp.Sign;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Storage;

namespace AnkrSDK.WalletConnect2
{
	public class WalletConnect2 : IWalletConnectable, IWalletConnectGenericRequester, IWalletConnectCommunicator, IQuittable, IPausable, IWalletConnectTransitionDataProvider
	{
		private const string SettingsFilenameString = "WalletConnect2Settings";
		public event Action<WalletConnect2TransitionBase> SessionStatusUpdated;
		//public event Action OnSend;
		public string SettingsFilename => SettingsFilenameString;
		public WalletConnect2Status Status { get; private set; }
		
		private WalletConnect2SettingsSO _settings;
		private WalletConnectSignClient _signClient;
		private SessionStruct? _sessionData;
		public bool ConnectionPending => Status != WalletConnect2Status.WalletConnected;

		public void Initialize(ScriptableObject settings)
		{
			Status = WalletConnect2Status.Uninitialized;
			
			_settings = settings as WalletConnect2SettingsSO;
			if (_settings == null)
			{
				var typeStr = settings == null ? "null" : settings.GetType().Name;
				Debug.LogError("WalletConnect: Could not initialize because settings are " + typeStr);
			}

			var prevStatus = Status;
			Status = WalletConnect2Status.Disconnected;
			SessionStatusUpdated?.Invoke(new WalletConnectInitialized(this, prevStatus, Status));

		}

		public async UniTask Connect()
		{
			var dappOptions = GenerateSignClientOptions();
			var dappConnectOptions = GenerateDappConnectOptions();

			if (_signClient == null)
			{
				_signClient = await WalletConnectSignClient.Init(dappOptions);
				Subscribe(_signClient);
			}
			
			var connectData = await _signClient.Connect(dappConnectOptions);

			var prevStatus = Status;
			Status = WalletConnect2Status.ConnectionRequestSent;
			SessionStatusUpdated?.Invoke(new SessionRequestSentTransition(this, prevStatus, Status));
			
			_sessionData = await connectData.Approval;

			prevStatus = Status;
			Status = WalletConnect2Status.WalletConnected;
			SessionStatusUpdated?.Invoke(new WalletConnectedTransition(this, prevStatus, Status));

		}
		
		public async UniTask<GenericJsonRpcResponse> GenericRequest(GenericJsonRpcRequest genericRequest)
		{
			CheckIfSessionCreated();

			if (genericRequest.RawParameters == null)
			{
				Debug.LogError("Can't have null raw parameters in SendGeneric of WalletConnect2");
				return new GenericJsonRpcResponse(new JObject());
			}

			var topic = _sessionData.Value.Topic;
			var method = genericRequest.Method;
			var genericResponseData = await _signClient.
				RequestWithMethod<object, GenericResponseData>(topic, genericRequest.RawParameters, method).
				AsUniTask();
			
			return genericResponseData.ToGenericRpcResponse();
		}

		public async UniTask<TResponseData> Send<TRequestData, TResponseData>(TRequestData data)
			where TRequestData : IIdentifiable 
			where TResponseData : IErrorHolder
		{
			CheckIfSessionCreated();

			var topic = _sessionData.Value.Topic; 
			var result = await _signClient.Request<TRequestData, TResponseData>(topic, data).AsUniTask();
			return result;
		}

		public async UniTask<string> EthSign(string address, string message)
		{
			CheckIfSessionCreated();
			if (!message.IsHex())
			{
				var rawMessage = Encoding.UTF8.GetBytes(message);

				var byteList = new List<byte>();
				var bytePrefix = "0x19".HexToByteArray();
				var textBytePrefix = Encoding.UTF8.GetBytes("Ethereum Signed Message:\n" + rawMessage.Length);

				byteList.AddRange(bytePrefix);
				byteList.AddRange(textBytePrefix);
				byteList.AddRange(rawMessage);

				var hash = new Sha3Keccack().CalculateHash(byteList.ToArray());

				message = "0x" + hash.ToHex();
			}
			
			Debug.Log(message);

			var request = new EthSignRequestData(address, message);
			
			var topic = _sessionData.Value.Topic;
			var response = await _signClient.Request<EthSignRequestData, EthResponseData>(topic, request);
			return response.Result;
		}

		public UniTask<string> EthPersonalSign(string address, string message)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<string> EthSignTypedData<T>(string address, T data, EIP712Domain eip712Domain)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<string> EthSendTransaction(params TransactionData[] transaction)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<string> EthSignTransaction(params TransactionData[] transaction)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<string> EthSendRawTransaction(string data, Encoding messageEncoding = null)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<string> WalletAddEthChain(EthChainData chainData)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<string> WalletSwitchEthChain(EthChain chainData)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<string> WalletUpdateEthChain(EthUpdateChainData chainData)
		{
			throw new System.NotImplementedException();
		}

		public async void Dispose()
		{
			await Disconnect();
		}

		public UniTask Quit()
		{
			return Disconnect();
		}

		public async UniTask OnApplicationPause(bool pauseStatus)
		{
			if (Status == WalletConnect2Status.Uninitialized)
			{
				throw new InvalidOperationException("WalletConnect is not initialized");
			}

			if (pauseStatus)
			{
				await Disconnect();
			}
			else
			{
				await Connect();
			}
		}
		
		private async UniTask Disconnect()
		{
			if (_signClient == null)
			{
				return;
			}

			if (ConnectionPending)
			{
				return;
			}

			if (_sessionData == null)
			{
				return;
			}

			var topic = _sessionData.Value.Topic;

			var errorResponse = ErrorResponse.FromErrorType(ErrorType.GENERIC);
			await _signClient.Disconnect(topic, errorResponse).AsUniTask();
			_sessionData = null;
			var prevStatus = Status;
			Status = WalletConnect2Status.Disconnected;
			SessionStatusUpdated?.Invoke(new WalletConnectedTransition(this, prevStatus, Status));

		}

		private void CheckIfSessionCreated()
		{
			if (_sessionData == null)
			{
				Debug.LogError("Session is not found in WalletConnect2");
			}
		}

		private void Subscribe(WalletConnectSignClient signClient)
		{
			var events = signClient.Events;
			events.ListenFor(EngineEvents.SessionExpire, OnSessionExpire);
			events.ListenFor(EngineEvents.SessionProposal, OnSessionProposal);
			events.ListenFor(EngineEvents.SessionConnect, OnSessionConnect);
			events.ListenFor(EngineEvents.SessionUpdate, OnSessionUpdate);
			events.ListenFor(EngineEvents.SessionExtend, OnSessionExtent);
			events.ListenFor(EngineEvents.SessionPing, OnSessionPing);
			events.ListenFor(EngineEvents.SessionDelete, OnSessionDelete);
			events.ListenFor(EngineEvents.SessionRequest, OnSessionRequest);
			events.ListenFor(EngineEvents.SessionEvent, OnSessionEvent);
		}

		private SignClientOptions GenerateSignClientOptions()
		{
			var dappFilePath = Path.Combine(Application.dataPath, ".wc", _settings.DappFileName);
			var signClientOptions = new SignClientOptions
			{
				ProjectId = _settings.ProjectId,
				Metadata = new global::WalletConnectSharp.Core.Models.Pairing.Metadata()
				{
					Description = _settings.Description,
					Icons = _settings.Icons,
					Name = _settings.Name,
					Url = _settings.Url
				},
				Storage = new FileSystemStorage(dappFilePath)
			};

			return signClientOptions;
		}

		private ConnectOptions GenerateDappConnectOptions()
		{
			var dappConnectOptions = new ConnectOptions()
			{
				RequiredNamespaces = new RequiredNamespaces(){}
			};

			foreach (var blockchainParams in _settings.BlockchainParameters)
			{
				var blockchainId = blockchainParams.BlockchainId;
				dappConnectOptions.RequiredNamespaces.Add(blockchainId, blockchainParams.ToRequiredNamespace());
			}

			return dappConnectOptions;
		}
		
		private void OnSessionExpire()
		{
			Debug.LogWarning("WalletConnect2 OnSessionExpire at " + Time.time);
		}

		private void OnSessionProposal()
		{
			Debug.LogWarning("WalletConnect2 OnSessionProposal at " + Time.time);
		}

		private void OnSessionConnect()
		{
			Debug.LogWarning("WalletConnect2 OnSessionConnect at " + Time.time);
		}

		private void OnSessionUpdate()
		{
			Debug.LogWarning("WalletConnect2 OnSessionUpdate at " + Time.time);
		}

		private void OnSessionExtent()
		{
			Debug.LogWarning("WalletConnect2 OnSessionExtent at " + Time.time);
		}
		
		private void OnSessionPing()
		{
			Debug.LogWarning("WalletConnect2 OnSessionPing at " + Time.time);
		}

		private void OnSessionDelete()
		{
			Debug.LogWarning("WalletConnect2 OnSessionDelete at " + Time.time);
		}

		private void OnSessionRequest()
		{
			Debug.LogWarning("WalletConnect2 OnSessionRequest at " + Time.time);
		}

		private void OnSessionEvent()
		{
			Debug.LogWarning("WalletConnect2 OnSessionEvent at " + Time.time);
		}
		
	}
}