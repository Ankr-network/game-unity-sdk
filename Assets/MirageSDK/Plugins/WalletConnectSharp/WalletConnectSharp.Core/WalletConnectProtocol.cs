using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using WalletConnectSharp.Core.Events;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Network;

namespace WalletConnectSharp.Core
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
		protected string _bridgeUrl;
		protected string _key;
		protected byte[] _keyRaw;
		protected List<string> _activeTopics = new List<string>();

		public event EventHandler<WalletConnectProtocol> OnTransportConnect;
		public event EventHandler<WalletConnectProtocol> OnTransportDisconnect;
		public event EventHandler<WalletConnectProtocol> OnTransportOpen;

		public bool SessionConnected { get; protected set; }

		public bool Disconnected { get; protected set; }

		public bool Connected => SessionConnected && TransportConnected;

		public bool Connecting { get; protected set; }

		public bool TransportConnected => Transport is { Connected: true } && Transport?.URL == _bridgeUrl;

		public ITransport Transport { get; private set; }

		public ICipher Cipher { get; private set; }

		public ClientMeta DappMetadata { get; set; }

		public ClientMeta WalletMetadata { get; set; }

		public ReadOnlyCollection<string> ActiveTopics => _activeTopics.AsReadOnly();

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

			transport ??= TransportFactory.Instance.BuildDefaultTransport(eventDelegator);

			_bridgeUrl = savedSession.BridgeURL;
			Transport = transport;

			cipher ??= new AESCipher();

			Cipher = cipher;

			_keyRaw = savedSession.KeyRaw;

			//Convert hex 
			_key = savedSession.Key;

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
			eventDelegator ??= new EventDelegator();

			Events = eventDelegator;

			transport ??= TransportFactory.Instance.BuildDefaultTransport(eventDelegator);

			Transport = transport;

			cipher ??= new AESCipher();

			Cipher = cipher;
		}

		protected async Task SetupTransport()
		{
			Transport.MessageReceived += TransportOnMessageReceived;
			Transport.OpenReceived += OnTransportOpenReceived;
			Transport.Closed += OnTransportClosed;

			await Transport.Open(_bridgeUrl);

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

		public virtual async Task Connect()
		{
			await SetupTransport();
		}

		public async Task SubscribeAndListenToTopic(string topic)
		{
			await Transport.Subscribe(topic);

			ListenToTopic(topic);
		}

		public void ListenToTopic(string topic)
		{
			if (!_activeTopics.Contains(topic))
			{
				_activeTopics.Add(topic);
			}
		}

		private async void TransportOnMessageReceived(object sender, MessageReceivedEventArgs e)
		{
			var networkMessage = e.Message;

			if (!_activeTopics.Contains(networkMessage.Topic))
			{
				return;
			}

			var encryptedPayload = JsonConvert.DeserializeObject<EncryptedPayload>(networkMessage.Payload);

			var json = await Cipher.DecryptWithKey(_keyRaw, encryptedPayload);

			var response = JsonConvert.DeserializeObject<JsonRpcResponse>(json);

			var wasResponse = false;
			if (response is { Event: { } })
			{
				wasResponse = Events.Trigger(response.Event, json);
			}

			if (wasResponse)
			{
				return;
			}

			var request = JsonConvert.DeserializeObject<JsonRpcRequest>(json);

			if (request is { Method: { } })
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

			string json = JsonConvert.SerializeObject(requestObject);

			var encrypted = await Cipher.EncryptWithKey(_keyRaw, json);

			if (sendingTopic == null)
			{
				sendingTopic = PeerId;
			}

			var message = new NetworkMessage()
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
			if (Transport != null)
			{
				Transport.Dispose();
				Transport = null;
			}
		}

		public virtual async Task Disconnect()
		{
			await DisconnectTransport();
		}
	}
}