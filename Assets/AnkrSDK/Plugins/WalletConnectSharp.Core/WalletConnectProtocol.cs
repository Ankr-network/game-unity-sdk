using System;
using System.Collections.Generic;
using System.Linq;
using AnkrSDK.WalletConnect.VersionShared.Models;
using AnkrSDK.WalletConnectSharp.Core.Events;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Network;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Core
{
	public abstract class WalletConnectProtocol : IDisposable
	{
		public static readonly string[] SigningMethods =
		{
			"eth_sendTransaction", 
			"eth_signTransaction", 
			"eth_sign", 
			"eth_signTypedData", 
			"eth_signTypedData_v1", 
			"eth_signTypedData_v2", 
			"eth_signTypedData_v3", 
			"eth_signTypedData_v4", 
			"personal_sign", 
			"wallet_silentSendTransaction",
			"wallet_silentSignMessage", 
			"wallet_addEthereumChain", 
			"wallet_switchEthereumChain", 
			"wallet_updateEthereumChain"
		};

		protected readonly EventDelegator EventDelegator;

		protected string Version = "1";
		protected string BridgeUrl;
		protected string Key;
		protected byte[] KeyRaw;
		protected readonly List<string> ActiveTopics = new List<string>();
		public bool CanSendRequests => Status == WalletConnectStatus.WalletConnected;
		public event EventHandler<WalletConnectProtocol> OnTransportConnect;
		public event EventHandler<WalletConnectProtocol> OnTransportDisconnect;
		public event EventHandler<WalletConnectProtocol> OnTransportOpen;
		public bool ConnectionPending => Status != WalletConnectStatus.WalletConnected;

		public WalletConnectStatus Status
		{
			get
			{
				if (TransportConnected)
				{
					if (WalletConnected)
					{
						return  WalletConnectStatus.WalletConnected;
					}

					return WaitingForSessionRequestResponse ? WalletConnectStatus.SessionRequestSent : WalletConnectStatus.TransportConnected;
				}

				return WalletConnected ? WalletConnectStatus.DisconnectedSessionCached : WalletConnectStatus.DisconnectedNoSession;
			}
		}

		public bool Connecting { get; protected set; }
		public ITransport Transport { get; private set; }
		public ClientMeta DappMetadata { get; protected set; }
		public ClientMeta WalletMetadata { get; protected set; }
		public string PeerId { get; protected set; }
		public string KeyData => Key;
		protected bool TransportConnected => Transport?.Connected == true && Transport?.URL == BridgeUrl;
		protected bool WaitingForSessionRequestResponse { get; set; }
		protected bool WalletConnected { get; set; }
		private readonly ICipher _cipher;

		/// <summary>
		///     Create a new WalletConnectProtocol object using a SavedSession as the session data. This will effectively resume
		///     the session, as long as the session data is valid
		/// </summary>
		/// <param name="savedSession">The SavedSession data to use. Cannot be null</param>
		/// <param name="transport">
		///     The transport interface to use for sending/receiving messages, null will result in the default
		///     transport being used
		/// </param>
		/// <param name="cipher">
		///     The cipher to use for encrypting and decrypting payload data, null will result in AESCipher being
		///     used
		/// </param>
		/// <param name="eventDelegator">The EventDelegator class to use, null will result in the default being used</param>
		/// <exception cref="ArgumentException">If a null SavedSession object was given</exception>
		protected WalletConnectProtocol(SavedSession savedSession, ITransport transport = null,
			ICipher cipher = null, EventDelegator eventDelegator = null)
		{
			if (savedSession == null)
			{
				throw new ArgumentException("savedSession cannot be null");
			}

			if (eventDelegator == null)
			{
				eventDelegator = new EventDelegator();
			}

			EventDelegator = eventDelegator;

			//TODO Do we need this for resuming?
			//_handshakeTopic = topicGuid.ToString();

			transport = transport ?? TransportFactory.Instance.BuildDefaultTransport(eventDelegator);

			BridgeUrl = savedSession.BridgeURL;
			SetTransport(transport);

			cipher = cipher ?? new AESCipher();

			_cipher = cipher;

			KeyRaw = savedSession.KeyRaw;

			//Convert hex 
			Key = savedSession.Key;

			PeerId = savedSession.PeerID;

			/*Transport.Open(this._bridgeUrl).ContinueWith(delegate(Task task)
			{
			    Transport.Subscribe(savedSession.ClientID);
			});

			this.Connected = true;*/
		}

		/// <summary>
		///     Create a new WalletConnectProtocol object and create a new dApp session.
		/// </summary>
		/// <param name="clientMeta">The metadata to send to wallets</param>
		/// <param name="transport">
		///     The transport interface to use for sending/receiving messages, null will result in the default
		///     transport being used
		/// </param>
		/// <param name="cipher">
		///     The cipher to use for encrypting and decrypting payload data, null will result in AESCipher being
		///     used
		/// </param>
		/// <param name="chainId">The chainId this dApp is using</param>
		/// <param name="bridgeUrl">The bridgeURL to use to communicate with the wallet</param>
		/// <param name="eventDelegator">The EventDelegator class to use, null will result in the default being used</param>
		/// <exception cref="ArgumentException">If an invalid ClientMeta object was given</exception>
		protected WalletConnectProtocol(ITransport transport = null,
			ICipher cipher = null,
			EventDelegator eventDelegator = null
		)
		{
			eventDelegator = eventDelegator ?? new EventDelegator();

			EventDelegator = eventDelegator;

			transport = transport ?? TransportFactory.Instance.BuildDefaultTransport(eventDelegator);

			SetTransport(transport);

			cipher = cipher ?? new AESCipher();

			_cipher = cipher;
		}

		protected async UniTask OpenTransport()
		{
			await Transport.Open(BridgeUrl);

			Debug.Log("[WalletConnect] Transport Opened");

			OnTransportConnect?.Invoke(this, this);
		}

		private void SetTransport(ITransport transport)
		{
			Transport = transport;
			Debug.Log("[WalletConnect] Transport Subscribed");
			Transport.MessageReceived += TransportOnMessageReceived;
			Transport.OpenReceived += OnTransportOpenReceived;
			Transport.Closed += OnTransportClosed;
		}

		private void OnTransportClosed(object sender, MessageReceivedEventArgs e)
		{
			OnTransportDisconnect?.Invoke(sender, this);
		}

		private void OnTransportOpenReceived(object sender, MessageReceivedEventArgs e)
		{
			OnTransportOpen?.Invoke(sender, this);
		}

		protected async UniTask DisconnectTransport()
		{
			Debug.Log("[WalletConnect] Transport UNSbuscribed");

			Transport.MessageReceived -= TransportOnMessageReceived;
			Transport.OpenReceived -= OnTransportOpenReceived;
			Transport.Closed -= OnTransportClosed;

			await Transport.Close();

			OnTransportDisconnect?.Invoke(this, this);
		}

		public async UniTask SubscribeAndListenToTopic(string topic)
		{
			await Transport.Subscribe(topic);

			ListenToTopic(topic);
		}

		public void ListenToTopic(string topic)
		{
			if (!ActiveTopics.Contains(topic))
			{
				ActiveTopics.Add(topic);
			}
		}

		private async void TransportOnMessageReceived(object sender, MessageReceivedEventArgs e)
		{
			var networkMessage = e.Message;

			if (!ActiveTopics.Contains(networkMessage.Topic))
			{
				return;
			}

			var encryptedPayload = JsonConvert.DeserializeObject<EncryptedPayload>(networkMessage.Payload);

			var json = await _cipher.DecryptWithKey(KeyRaw, encryptedPayload);

			var response = JsonConvert.DeserializeObject<JsonRpcResponse>(json);

			var wasResponse = false;
			if (response?.Event != null)
			{
				wasResponse = EventDelegator.Trigger(response.Event, json);
			}

			if (wasResponse)
			{
				return;
			}

			var request = JsonConvert.DeserializeObject<JsonRpcRequest>(json);

			if (request?.Method != null)
			{
				EventDelegator.Trigger(request.Method, json);
			}
		}

		protected async UniTask SendRequest<T>(T requestObject, string sendingTopic = null,
			bool? forcePushNotification = null)
		{
			bool silent;
			if (forcePushNotification != null)
			{
				silent = (bool)!forcePushNotification;
			}
			else if (requestObject is JsonRpcRequest request)
			{
				silent = request.Method.StartsWith("wc_") || !SigningMethods.Contains(request.Method);
			}
			else
			{
				silent = false;
			}

			var json = JsonConvert.SerializeObject(requestObject);

			var encrypted = await _cipher.EncryptWithKey(KeyRaw, json);

			if (sendingTopic == null)
			{
				sendingTopic = PeerId;
			}

			var message = new NetworkMessage
			{
				Payload = JsonConvert.SerializeObject(encrypted), 
				Silent = silent,
				Topic = sendingTopic, 
				Type = "pub"
			};

			await Transport.SendMessage(message);
		}

		public void Dispose()
		{
			if (Transport == null)
			{
				return;
			}

			Transport.Dispose();
			Transport = null;
		}
	}
}