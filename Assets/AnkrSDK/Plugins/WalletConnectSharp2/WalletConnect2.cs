using System;
using System.IO;
using System.Text;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Infrastructure;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum.Types;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WalletConnectSharp.Events.Model;
using WalletConnectSharp.Sign;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Sign.Models.Engine.Events;
using WalletConnectSharp.Storage;

namespace AnkrSDK.Plugins.WalletConnectSharp2
{
	public class WalletConnect2 : IWalletConnectable, IWalletConnectCommunicator, IQuittable, IPausable, IUpdatable 
	{
		private const string SettingsFilenameString = "WalletConnectSettings";
		public string SettingsFilename => SettingsFilenameString;
		
		private bool _initialized;
		private WalletConnect2SettingsSO _settings;
		private WalletConnectSignClient _signClient;
		
		public void Initialize(ScriptableObject settings)
		{
			_settings = settings as WalletConnect2SettingsSO;
			if (_settings != null)
			{
				_initialized = true;
			}
			else
			{
				var typeStr = settings == null ? "null" : settings.GetType().Name;
				Debug.LogError("WalletConnect: Could not initialize because settings are " + typeStr);
			}
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
		
		private void OnSessionExpire()
		{
			
		}

		private void OnSessionProposal()
		{
			
		}

		private void OnSessionConnect()
		{
			
		}


		private void OnSessionUpdate()
		{
			
		}

		private void OnSessionExtent()
		{
			
		}
		
		private void OnSessionPing()
		{
			
		}

		private void OnSessionDelete()
		{
			
		}

		private void OnSessionRequest()
		{
			
		}

		private void OnSessionEvent()
		{
			
		}

		//TODO ANTON finish this
		private SignClientOptions GenerateSignClientOptions()
		{
			var dappFilePath = Path.Combine(Application.dataPath, ".wc", "store_dapp_example.json");
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

		//TODO ANTON finish this
		private ConnectOptions GenerateDappConnectOptions()
		{
			var dappConnectOptions = new ConnectOptions()
			{
				RequiredNamespaces = new RequiredNamespaces()
				{
					{
						"eip155", new RequiredNamespace()
						{
							Methods = new[]
							{
								"eth_sendTransaction",
								"eth_signTransaction",
								"eth_sign",
								"personal_sign",
								"eth_signTypedData",
							},
							Chains = new[]
							{
								"eip155:1"
							},
							Events = new[]
							{
								"chainChanged", 
								"accountsChanged",
							}
						}
					}
				}
			};

			return dappConnectOptions;
		}

		public WalletConnectStatus Status { get; }
		public UniTask<string> EthSign(string address, string message)
		{
			throw new System.NotImplementedException();
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

		public UniTask<TResponse> Send<TRequest, TResponse>(TRequest data) where TRequest : JsonRpcRequest where TResponse : JsonRpcResponse
		{
			throw new System.NotImplementedException();
		}

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}

		public UniTask Quit()
		{
			throw new System.NotImplementedException();
		}

		public UniTask OnApplicationPause(bool pauseStatus)
		{
			throw new System.NotImplementedException();
		}

		public void Update()
		{
			throw new System.NotImplementedException();
		}
	}
}