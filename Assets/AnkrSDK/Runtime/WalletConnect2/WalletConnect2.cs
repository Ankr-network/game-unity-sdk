using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AnkrSDK.WalletConnect.VersionShared;
using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.WalletConnect.VersionShared.Models;
using AnkrSDK.WalletConnect.VersionShared.Models.DeepLink;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum.Types;
using AnkrSDK.WalletConnect.VersionShared.Utils;
using AnkrSDK.WalletConnect2.Data;
using AnkrSDK.WalletConnect2.Events;
using AnkrSDK.WalletConnect2.RpcRequests.Eth;
using AnkrSDK.WalletConnect2.RpcResponses;
using AnkrSDK.WalletConnect2.RpcResponses.Eth;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using WalletConnectSharp.Common.Logging;
using WalletConnectSharp.Common.Model.Errors;
using WalletConnectSharp.Core.Models.Pairing;
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
		public event Action OnSend;
		public event Action<WalletConnect2TransitionBase> SessionStatusUpdated;
		public string SettingsFilename => SettingsFilenameString;
		public WalletConnect2Status Status { get; private set; }
		public bool Connecting { get; private set; }

		public string ConnectURL
		{
			get
			{
				if (_connectedData == null)
				{
					throw new InvalidDataException("ConnectData not found in WalletConnect2, it is probably not connected yet");
				}

				return _connectedData.Uri;
			}
		}
		public bool CanSendRequests => Status != WalletConnect2Status.WalletConnected;
		
		//TODO ANTON handle ChainId later
		public int ChainId => -1;
		//TODO ANTON handle Accounts later
		public string[] Accounts
		{
			get
			{
				if (_sessionData.HasValue)
				{
					//here we should convert selected wallet to CAIP namespace
					//to use as a Namespaces dictionary key
					return _sessionData.Value.Namespaces["eip155"].Accounts;
				}
				else
				{
					throw new InvalidDataException("SessionStruct not found in WalletConnect2, it is probably not connected yet");
				}
			}
		}

		private WalletConnect2SettingsSO _settings;
		private WalletConnectSignClient _signClient;
		private ConnectedData _connectedData;
		private SessionStruct? _sessionData;
		private WalletEntry _selectedWallet;

		public void Initialize(ScriptableObject settings)
		{
			WC2Debug.AddLogger(new WalletConnect2Logger());
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
			if (Connecting)
			{
				return;
			}

			if (Status.IsAny(WalletConnect2Status.AnythingConnected))
			{
				return;
			}
			
			var dappOptions = GenerateSignClientOptions();
			var dappConnectOptions = GenerateDappConnectOptions();
			
			Connecting = true;

			if (_signClient == null)
			{
				_signClient = await WalletConnectSignClient.Init(dappOptions);
				Subscribe(_signClient);
			}

			_connectedData = await _signClient.Connect(dappConnectOptions);

			SetupDefaultWallet().Forget();
			Connecting = false;
			
			Debug.Log("WalletConnect2: Connect finished, uri = " + _connectedData.Uri);

			
			var prevStatus = Status;
			Status = WalletConnect2Status.ConnectionRequestSent;
			SessionStatusUpdated?.Invoke(new SessionRequestSentTransition(this, prevStatus, Status));

			_sessionData = await _connectedData.Approval;
			
			var sessionDataJson = JsonConvert.SerializeObject(_sessionData);
			Debug.Log("WalletConnect2: Connect finished, sessionData = " + sessionDataJson);

			prevStatus = Status;
			Status = WalletConnect2Status.WalletConnected;
			SessionStatusUpdated?.Invoke(new WalletConnectedTransition(this, prevStatus, Status));
		}

		public async UniTask DisconnectSession(bool connectNewSession = true)
		{
			if (Status == WalletConnect2Status.Disconnected || Status == WalletConnect2Status.Uninitialized)
			{
				return;
			}
			
			await Disconnect();

			if (connectNewSession)
			{
				await Connect();
			}
		}

		public async UniTask<GenericJsonRpcResponse> GenericRequest(GenericJsonRpcRequest genericRequest)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			if (genericRequest.RawParameters == null)
			{
				Debug.LogError("Can't have null raw parameters in SendGeneric of WalletConnect2");
				return default;
			}

			var topic = _sessionData.Value.Topic;
			var method = genericRequest.Method;
			var genericResponseData = await _signClient.RequestWithMethod<object, GenericResponseData>(topic, genericRequest.RawParameters, method).AsUniTask();

			return genericResponseData.ToGenericRpcResponse();
		}

		public async UniTask OpenDeepLink(Wallets wallet)
		{
			_selectedWallet = await WalletDownloadHelper.FindWalletEntry(wallet);

			OpenDeepLink();
		}

		public void OpenDeepLink(WalletEntry selectedWallet)
		{
			_selectedWallet = selectedWallet;

			OpenDeepLink();
		}

		public void OpenDeepLink()
		{
			if (Status != WalletConnect2Status.ConnectionRequestSent)
			{
				Debug.LogError("WalletConnectUnity.ActiveSession not ready for a user prompt" +
				               "\nWait for Status == WalletConnect2Status.ConnectionRequestSent");
				return;
			}

			#if UNITY_ANDROID
			Application.OpenURL(ConnectURL);
			#elif UNITY_IOS
			if (_selectedWallet == null)
			{
				throw new NotImplementedException(
					"You must use OpenDeepLink(WalletEntry) or set _selectedWallet on iOS!");
			}

			var url = MobileWalletURLFormatHelper
				.GetURLForMobileWalletOpen(ConnectURL, _selectedWallet.mobile);

			Debug.Log("[WalletConnect] Opening URL: " + url);

			Application.OpenURL(url);
			#else
			Debug.Log("Platform does not support deep linking");
			return;
			#endif
		}

		public async UniTask OpenMobileWallet(Wallets wallet)
		{
			_selectedWallet = await WalletDownloadHelper.FindWalletEntry(wallet);
			OpenMobileWallet();
		}

		public void OpenMobileWallet(WalletEntry selectedWallet)
		{
			_selectedWallet = selectedWallet;

			OpenMobileWallet();
		}
		
		public void OpenMobileWallet()
		{
			#if UNITY_ANDROID
			var signingURL = ConnectURL.Split('@')[0];

			Application.OpenURL(signingURL);
			#elif UNITY_IOS
			if (_selectedWallet == null)
			{
				throw new NotImplementedException(
					"You must use OpenMobileWallet(WalletEntry) or set _selectedWallet on iOS!");
			}

			var url = MobileWalletURLFormatHelper
				.GetURLForMobileWalletOpen(ConnectURL, _selectedWallet.mobile).Split('?')[0];

			Debug.Log("Opening: " + url);
			Application.OpenURL(url);
			#else
			Debug.Log("Platform does not support deep linking");
			return;
			#endif
		}

		public async UniTask<TResponseData> Send<TRequestData, TResponseData>(TRequestData data)
			where TRequestData : IIdentifiable
			where TResponseData : IErrorHolder
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			var topic = _sessionData.Value.Topic;
			var result = await _signClient.Request<TRequestData, TResponseData>(topic, data).AsUniTask();
			return result;
		}

		public async UniTask<string> EthSign(string address, string message)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

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

		public async UniTask<string> EthPersonalSign(string address, string message)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			if (!message.IsHex())
			{
				message = "0x" + Encoding.UTF8.GetBytes(message).ToHex();
			}

			var request = new EthPersonalSignData(address, message);

			var topic = _sessionData.Value.Topic;
			var response = await _signClient.Request<EthPersonalSignData, EthResponseData>(topic, request);
			return response.Result;
		}

		public async UniTask<string> EthSignTypedData<T>(string address, T data, EIP712Domain eip712Domain)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			var request = new EthSignTypedData<T>(address, data, eip712Domain);
			var topic = _sessionData.Value.Topic;
			var response = await _signClient.Request<EthSignTypedData<T>, EthResponseData>(topic, request);

			return response.Result;
		}

		public async UniTask<string> EthSendTransaction(params TransactionData[] transaction)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			var request = new EthSendTransactionData(transaction);
			var topic = _sessionData.Value.Topic;
			var response = await _signClient.Request<EthSendTransactionData, EthResponseData>(topic, request);
			return response.Result;
		}

		public async UniTask<string> EthSignTransaction(params TransactionData[] transaction)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			var request = new EthSignTransactionData(transaction);
			var topic = _sessionData.Value.Topic;
			var response = await _signClient.Request<EthSignTransactionData, EthResponseData>(topic, request);

			return response.Result;
		}

		public async UniTask<string> EthSendRawTransaction(string data, Encoding messageEncoding = null)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			if (!data.IsHex())
			{
				var encoding = messageEncoding;
				if (encoding == null)
				{
					encoding = Encoding.UTF8;
				}

				data = "0x" + encoding.GetBytes(data).ToHex();
			}

			var request = new EthSendRawTransactionData(data);
			var topic = _sessionData.Value.Topic;
			var response = await _signClient.Request<EthSendRawTransactionData, EthResponseData>(topic, request, "eth_sendRawTransaction");

			return response.Result;
		}

		public async UniTask<string> WalletAddEthChain(EthChainData chainData)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			var request = new WalletAddEthChainData(chainData);
			var topic = _sessionData.Value.Topic;
			var response = await _signClient.Request<WalletAddEthChainData, EthResponseData>(topic, request);
			return response.Result;
		}

		public async UniTask<string> WalletSwitchEthChain(EthChain chainData)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			var request = new WalletSwitchEthChainData(chainData);
			var topic = _sessionData.Value.Topic;
			var response = await _signClient.Request<WalletSwitchEthChainData, EthResponseData>(topic, request);
			return response.Result;
		}

		public async UniTask<string> WalletUpdateEthChain(EthUpdateChainData chainData)
		{
			if (!CheckIfSessionCreated())
			{
				return default;
			}

			var request = new WalletUpdateEthChainData(chainData);
			var topic = _sessionData.Value.Topic;
			var response = await _signClient.Request<WalletUpdateEthChainData, EthResponseData>(topic, request);
			return response.Result;
		}

		public void Dispose()
		{
			Disconnect().Forget();
		}

		public UniTask Quit()
		{
			return Disconnect();
		}

		public UniTask OnApplicationPause(bool pauseStatus)
		{
			if (Status == WalletConnect2Status.Uninitialized)
			{
				throw new InvalidOperationException("WalletConnect is not initialized");
			}

			return pauseStatus ? Disconnect() : Connect();
		}

		private async UniTask SetupDefaultWallet()
		{
			if (_settings.DefaultWallet == Wallets.None)
			{
				return;
			}

			var walletEntry = await WalletDownloadHelper.FindWalletEntry(_settings.DefaultWallet);
			if (walletEntry != null)
			{
				_selectedWallet = walletEntry;
				await walletEntry.DownloadImages();
			}
		}

		private async UniTask Disconnect()
		{
			if (Status == WalletConnect2Status.Disconnected)
			{
				return;
			}

			if (_signClient == null || CanSendRequests || _sessionData == null)
			{
				SwitchToDisconnectedState();
				return;
			}

			var topic = _sessionData.Value.Topic;

			var errorResponse = ErrorResponse.FromErrorType(ErrorType.GENERIC);
			await _signClient.Disconnect(topic, errorResponse).AsUniTask();
			SwitchToDisconnectedState();
		}

		private void SwitchToDisconnectedState()
		{
			var prevStatus = Status;
			_sessionData = null;
			_connectedData = null;
			_signClient = null;
			Status = WalletConnect2Status.Disconnected;
			SessionStatusUpdated?.Invoke(new WalletDisconnectedTransition(this, prevStatus, Status));
		}

		private bool CheckIfSessionCreated()
		{
			if (_sessionData == null)
			{
				Debug.LogError("Session is not found in WalletConnect2");
				return false;
			}

			return true;
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
				Metadata = new Metadata
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
			var dappConnectOptions = new ConnectOptions
			{
				RequiredNamespaces = new RequiredNamespaces()
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
			OnSend?.Invoke();
		}

		private void OnSessionEvent()
		{
			Debug.LogWarning("WalletConnect2 OnSessionEvent at " + Time.time);
		}
	}
}