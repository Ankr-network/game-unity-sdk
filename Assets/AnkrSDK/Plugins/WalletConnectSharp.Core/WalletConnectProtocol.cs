using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Events;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Network;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Core
{
	public class WalletConnectProtocol : IDisposable
	{
		public static readonly string[] SigningMethods = {
			"eth_sendTransaction",
			"eth_signTransaction",
			"eth_sign",
			"eth_signTypedData",
			"eth_signTypedData_v1",
			"eth_signTypedData_v2",
			"eth_signTypedData_v3",
			"eth_signTypedData_v4",
			"personal_sign",
		};

		public readonly EventDelegator Events;

		protected string Version = "1";
		protected string BridgeUrl;
		protected string Key;
		protected byte[] KeyRaw;
		protected readonly List<string> ActiveTopics = new List<string>();

		public event EventHandler<WalletConnectProtocol> OnTransportConnect;
		public event EventHandler<WalletConnectProtocol> OnTransportDisconnect;
		public event EventHandler<WalletConnectProtocol> OnTransportOpen;

		public bool SessionConnected { get; protected set; }

		public bool Disconnected { get; protected set; }

		public bool Connected => SessionConnected && TransportConnected;

		public bool Connecting { get; protected set; }

		public bool TransportConnected => Transport?.Connected == true && Transport?.URL == BridgeUrl;

		public ITransport Transport { get; private set; }

		public ICipher Cipher { get; private set; }

		public ClientMeta DappMetadata { get; set; }

		public ClientMeta WalletMetadata { get; set; }

		public string PeerId { get; protected set; }


		/// <summary>
		/// Create a new WalletConnectProtocol object using a SavedSession as the session data. This will effectively resume
		/// the session, as long as the session data is valid
		/// </summary>
		/// <param name="savedSession">The SavedSession data to use. Cannot be null</param>
		/// <param name="transport">The transport interface to use for sending/receiving messages, null will result in the default transport being used</param>
		/// <param name="cipher">The cipher to use for encrypting and decrypting payload data, null will result in AESCipher being used</param>
		/// <param name="eventDelegator">The EventDelegator class to use, null will result in the default being used</param>
		/// <exception cref="ArgumentException">If a null SavedSession object was given</exception>
		public WalletConnectProtocol(SavedSession savedSession, ITransport transport = null,
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

			Events = eventDelegator;

			//TODO Do we need this for resuming?
			//_handshakeTopic = topicGuid.ToString();

			transport = transport ?? TransportFactory.Instance.BuildDefaultTransport(eventDelegator);

			BridgeUrl = savedSession.BridgeURL;
			Transport = transport;

			cipher = cipher ?? new AESCipher();

			Cipher = cipher;

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
		/// Create a new WalletConnectProtocol object and create a new dApp session.
		/// </summary>
		/// <param name="clientMeta">The metadata to send to wallets</param>
		/// <param name="transport">The transport interface to use for sending/receiving messages, null will result in the default transport being used</param>
		/// <param name="cipher">The cipher to use for encrypting and decrypting payload data, null will result in AESCipher being used</param>
		/// <param name="chainId">The chainId this dApp is using</param>
		/// <param name="bridgeUrl">The bridgeURL to use to communicate with the wallet</param>
		/// <param name="eventDelegator">The EventDelegator class to use, null will result in the default being used</param>
		/// <exception cref="ArgumentException">If an invalid ClientMeta object was given</exception>
		public WalletConnectProtocol(ITransport transport = null,
			ICipher cipher = null,
			EventDelegator eventDelegator = null
		)
		{
			eventDelegator = eventDelegator ?? new EventDelegator();

			Events = eventDelegator;

			transport = transport ?? TransportFactory.Instance.BuildDefaultTransport(eventDelegator);

			Transport = transport;

			cipher = cipher ?? new AESCipher();

			Cipher = cipher;
		}

		protected async Task SetupTransport()
		{
			Transport.MessageReceived += TransportOnMessageReceived;
			Transport.OpenReceived += OnTransportOpenReceived;
			Transport.Closed += OnTransportClosed;

			await Transport.Open(BridgeUrl);

			Debug.Log("[WalletConnect] Transport Opened");

			TriggerOnTransportConnect();
		}

		private void OnTransportClosed(object sender, MessageReceivedEventArgs e)
		{
			OnTransportDisconnect?.Invoke(sender, this);
		}

		private void OnTransportOpenReceived(object sender, MessageReceivedEventArgs e)
		{
			OnTransportOpen?.Invoke(sender, this);
		}

		protected async Task DisconnectTransport()
		{
			await Transport.Close();

			Transport.MessageReceived -= TransportOnMessageReceived;
			Transport.OpenReceived -= OnTransportOpenReceived;
			Transport.Closed -= OnTransportClosed;

			OnTransportDisconnect?.Invoke(this, this);
		}

		protected virtual void TriggerOnTransportConnect()
		{
			OnTransportConnect?.Invoke(this, this);
		}

		public virtual Task Connect()
		{
			return SetupTransport();
		}

		public async Task SubscribeAndListenToTopic(string topic)
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

			var json = await Cipher.DecryptWithKey(KeyRaw, encryptedPayload);

			var response = JsonConvert.DeserializeObject<JsonRpcResponse>(json);

			var wasResponse = false;
			if (response?.Event != null)
			{
				wasResponse = Events.Trigger(response.Event, json);
			}

			if (wasResponse)
			{
				return;
			}

			var request = JsonConvert.DeserializeObject<JsonRpcRequest>(json);

			if (request?.Method != null)
			{
				Events.Trigger(request.Method, json);
			}
		}

		public async Task SendRequest<T>(T requestObject, string sendingTopic = null,
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

			var encrypted = await Cipher.EncryptWithKey(KeyRaw, json);

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

		public virtual Task Disconnect()
		{
			return DisconnectTransport();
		}
	}
}