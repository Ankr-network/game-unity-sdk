using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Events;
using AnkrSDK.WalletConnectSharp.Core.Events.Model;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum.Types;
using AnkrSDK.WalletConnectSharp.Core.Network;
using AnkrSDK.WalletConnectSharp.Core.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Core
{
	public class WalletConnectSession : WalletConnectProtocol
	{
		private const string ConnectTopic = "connect";
		private const string DisconnectTopic = "disconnect";
		private const string SessionFailedTopic = "session_failed";

		public event EventHandler<WalletConnectSession> OnSessionConnect;
		public event EventHandler<WalletConnectSession> OnSessionCreated;
		public event EventHandler<WalletConnectSession> OnSessionResumed;
		public event EventHandler OnSessionDisconnect;
		public event EventHandler<WalletConnectSession> OnSend;
		public event EventHandler<WCSessionData> SessionUpdate;

		public int NetworkId { get; private set; }

		public string[] Accounts { get; private set; }

		public bool ReadyForUserPrompt { get; private set; }

		public bool SessionUsed { get; private set; }

		public int ChainId { get; private set; }

		private string _clientId = "";

		private string _handshakeTopic;

		private long _handshakeId;

		public string URI
		{
			get
			{
				var topicEncode = WebUtility.UrlEncode(_handshakeTopic);
				var versionEncode = WebUtility.UrlEncode(Version);
				var bridgeUrlEncode = WebUtility.UrlEncode(BridgeUrl);
				var keyEncoded = WebUtility.UrlEncode(Key);

				return "wc:" + topicEncode + "@" + versionEncode + "?bridge=" + bridgeUrlEncode + "&key=" + keyEncoded;
			}
		}

		protected WalletConnectSession(SavedSession savedSession, ITransport transport = null, ICipher cipher = null,
			EventDelegator eventDelegator = null) : base(savedSession, transport, cipher, eventDelegator)
		{
			DappMetadata = savedSession.DappMeta;
			WalletMetadata = savedSession.WalletMeta;
			ChainId = savedSession.ChainID;

			_clientId = savedSession.ClientID;

			Accounts = savedSession.Accounts;

			NetworkId = savedSession.NetworkID;

			_handshakeId = savedSession.HandshakeID;
			SubscribeForSessionResponse();

			SessionConnected = true;
		}

		protected WalletConnectSession(ClientMeta clientMeta, string bridgeUrl = null, ITransport transport = null,
			ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null) : base(transport, cipher,
			eventDelegator)
		{
			if (clientMeta == null)
			{
				throw new ArgumentException("clientMeta cannot be null!");
			}

			if (string.IsNullOrWhiteSpace(clientMeta._description))
			{
				throw new ArgumentException("clientMeta must include a valid Description");
			}

			if (string.IsNullOrWhiteSpace(clientMeta._name))
			{
				throw new ArgumentException("clientMeta must include a valid Name");
			}

			if (string.IsNullOrWhiteSpace(clientMeta._url))
			{
				throw new ArgumentException("clientMeta must include a valid URL");
			}

			if (clientMeta._icons == null || clientMeta._icons.Length == 0)
			{
				throw new ArgumentException(
					"clientMeta must include an array of Icons the Wallet app can use. These Icons must be URLs to images. You must include at least one image URL to use");
			}

			if (bridgeUrl == null)
			{
				bridgeUrl = DefaultBridge.ChooseRandomBridge();
			}

			bridgeUrl = DefaultBridge.GetBridgeUrl(bridgeUrl);

			if (bridgeUrl.StartsWith("https"))
			{
				bridgeUrl = bridgeUrl.Replace("https", "wss");
			}
			else if (bridgeUrl.StartsWith("http"))
			{
				bridgeUrl = bridgeUrl.Replace("http", "ws");
			}

			DappMetadata = clientMeta;
			ChainId = chainId;
			BridgeUrl = bridgeUrl;

			SessionConnected = false;

			CreateNewSession();
		}

		private void CreateNewSession()
		{
			if (SessionConnected)
			{
				throw new IOException(
					"You cannot create a new session after connecting the session. Create a new WalletConnectSession object to create a new session");
			}

			BridgeUrl = DefaultBridge.GetBridgeUrl(BridgeUrl);

			var topicGuid = Guid.NewGuid();

			_handshakeTopic = topicGuid.ToString();

			_clientId = Guid.NewGuid().ToString();

			GenerateKey();

			Transport?.ClearSubscriptions();

			SessionUsed = false;
			ReadyForUserPrompt = false;
		}

		private void EnsureNotDisconnected()
		{
			if (Disconnected)
			{
				throw new IOException(
					"Session stale! The session has been disconnected. This session cannot be reused.");
			}
		}

		private void GenerateKey()
		{
			//Generate a random secret
			var secret = new byte[32];
			var rngCsp = new RNGCryptoServiceProvider();
			rngCsp.GetBytes(secret);

			KeyRaw = secret;

			//Convert hex 
			Key = KeyRaw.ToHex().ToLower();
		}

		public virtual async Task<WCSessionData> ConnectSession()
		{
			EnsureNotDisconnected();

			Connecting = true;
			try
			{
				if (!TransportConnected)
				{
					await SetupTransport();
				}
				else
				{
					Debug.Log("Transport already connected. No need to setup");
				}

				ReadyForUserPrompt = false;
				await SubscribeAndListenToTopic(_clientId);

				ListenToTopic(_handshakeTopic);

				WCSessionData result;
				if (!SessionConnected)
				{
					result = await CreateSession();
					//Reset this back after we have established a session
					ReadyForUserPrompt = false;
					Connecting = false;

					OnSessionCreated?.Invoke(this, this);
				}
				else
				{
					result = new WCSessionData
					{
						accounts = Accounts,
						approved = true,
						chainId = ChainId,
						networkId = NetworkId,
						peerId = PeerId,
						peerMeta = WalletMetadata
					};
					Connecting = false;

					OnSessionResumed?.Invoke(this, this);
				}

				OnSessionConnect?.Invoke(this, this);

				return result;
			}
			catch (IOException e)
			{
				//If the transport is connected, then disconnect that
				//we tried our best, they can try again
				if (TransportConnected)
				{
					await DisconnectTransport();
				}
				else
				{
					throw new IOException("Transport Connection failed", e);
				}

				throw new IOException("Session Connection failed", e);
			}
			finally
			{
				//The session has been made, we are no longer ready for another user prompt
				ReadyForUserPrompt = false;
				Connecting = false;
			}
		}

		public override async Task Connect()
		{
			EnsureNotDisconnected();

			await base.Connect();

			await ConnectSession();
		}

		public async Task DisconnectSession(string disconnectMessage = "Session Disconnected")
		{
			EnsureNotDisconnected();

			var request = new WCSessionUpdate(new WCSessionData
			{
				approved = false,
				chainId = 0,
				accounts = null,
				networkId = 0
			});

			await SendRequest(request);

			await base.Disconnect();

			HandleSessionDisconnect(disconnectMessage);
		}

		public override Task Disconnect()
		{
			return DisconnectSession();
		}

		public async Task<string> EthSign(string address, string message)
		{
			EnsureNotDisconnected();

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

			var request = new EthSign(address, message);

			var response = await Send<EthSign, EthResponse>(request);

			return response.Result;
		}

		public async Task<string> EthPersonalSign(string address, string message)
		{
			EnsureNotDisconnected();

			if (!message.IsHex())
			{
				/*var rawMessage = Encoding.UTF8.GetBytes(message);
				
				var byteList = new List<byte>();
				var bytePrefix = "0x19".HexToByteArray();
				var textBytePrefix = Encoding.UTF8.GetBytes("Ethereum Signed Message:\n" + rawMessage.Length);

				byteList.AddRange(bytePrefix);
				byteList.AddRange(textBytePrefix);
				byteList.AddRange(rawMessage);
				
				var hash = new Sha3Keccack().CalculateHash(byteList.ToArray());*/

				message = "0x" + Encoding.UTF8.GetBytes(message).ToHex();
			}

			var request = new EthPersonalSign(address, message);

			var response = await Send<EthPersonalSign, EthResponse>(request);


			return response.Result;
		}

		public async Task<string> EthSignTypedData<T>(string address, T data, EIP712Domain eip712Domain)
		{
			EnsureNotDisconnected();

			var request = new EthSignTypedData<T>(address, data, eip712Domain);

			var response = await Send<EthSignTypedData<T>, EthResponse>(request);

			return response.Result;
		}

		public async Task<string> EthSendTransaction(params TransactionData[] transaction)
		{
			EnsureNotDisconnected();
			var request = new EthSendTransaction(transaction);
			var response = await Send<EthSendTransaction, EthResponse>(request);
			return response.Result;
		}

		public async Task<string> EthSignTransaction(params TransactionData[] transaction)
		{
			EnsureNotDisconnected();

			var request = new EthSignTransaction(transaction);

			var response = await Send<EthSignTransaction, EthResponse>(request);

			return response.Result;
		}

		public async Task<string> EthSendRawTransaction(string data, Encoding messageEncoding = null)
		{
			EnsureNotDisconnected();

			if (!data.IsHex())
			{
				var encoding = messageEncoding;
				if (encoding == null)
				{
					encoding = Encoding.UTF8;
				}

				data = "0x" + encoding.GetBytes(data).ToHex();
			}

			var request = new EthGenericRequest<string>("eth_sendRawTransaction", data);

			var response = await Send<EthGenericRequest<string>, EthResponse>(request);

			return response.Result;
		}

		public async Task<TResponse> Send<TRequest, TResponse>(TRequest data)
			where TRequest : JsonRpcRequest
			where TResponse : JsonRpcResponse
		{
			EnsureNotDisconnected();

			var eventCompleted = new TaskCompletionSource<TResponse>(TaskCreationOptions.None);

			void HandleResponse(object sender, JsonRpcResponseEvent<TResponse> @event)
			{
				var response = @event.Response;
				if (response.IsError)
				{
					eventCompleted.TrySetException(new IOException(response.Error.Message));
				}
				else
				{
					eventCompleted.TrySetResult(@event.Response);
				}
			}

			Events.ListenForResponse<TResponse>(data.ID, HandleResponse);

			await SendRequest(data);

			OnSend?.Invoke(this, this);

			return await eventCompleted.Task;
		}

		/// <summary>
		/// Create a new WalletConnect session with a Wallet.
		/// </summary>
		/// <returns></returns>
		private async Task<WCSessionData> CreateSession()
		{
			EnsureNotDisconnected();

			var data = new WcSessionRequest(DappMetadata, _clientId, ChainId);

			_handshakeId = data.ID;
			SubscribeForSessionResponse();

			await SendRequest(data, _handshakeTopic);

			SessionUsed = true;

			var sessionCompletionSource = new TaskCompletionSource<WCSessionData>(TaskCreationOptions.None);

			SubscribeOnConnectMessage(sessionCompletionSource);

			SubscribeOnFailedMessage(sessionCompletionSource);

			ReadyForUserPrompt = true;

			Debug.Log("[WalletConnect] Session Ready for Wallet");

			var response = await sessionCompletionSource.Task;

			ReadyForUserPrompt = false;

			return response;
		}

		private void SubscribeOnFailedMessage(TaskCompletionSource<WCSessionData> sessionCompletionSource)
		{
			Events.ListenFor(SessionFailedTopic,
				(object sender, GenericEvent<ErrorResponse> @event) =>
				{
					if (@event.Response.Message == "Not Approved" || @event.Response.Message == "Session Rejected")
					{
						sessionCompletionSource.TrySetCanceled();
					}
					else
					{
						sessionCompletionSource.TrySetException(
							new IOException("WalletConnect: Session Failed: " + @event.Response.Message));
					}
				});
		}

		private void SubscribeOnConnectMessage(TaskCompletionSource<WCSessionData> eventCompleted)
		{
			void HandleConnectMessage(object _, GenericEvent<WCSessionData> @event)
			{
				eventCompleted.TrySetResult(@event.Response);
			}

			Events.ListenFor<WCSessionData>(ConnectTopic, HandleConnectMessage);
		}

		private void SubscribeForSessionUpdates()
		{
			Events.ListenFor(WCSessionUpdate.SessionUpdateMethod,
				(object _, GenericEvent<WCSessionUpdate> @event) =>
					HandleSessionUpdate(@event.Response.parameters[0]));
		}

		private void SubscribeForSessionResponse()
		{
			void HandleSessionResponse(object sender, JsonRpcResponseEvent<WCSessionRequestResponse> jsonresponse)
			{
				var response = jsonresponse.Response.result;

				if (response?.approved == true)
				{
					HandleSessionUpdate(response);
				}
				else
				{
					HandleSessionDisconnect(
						jsonresponse.Response.IsError ? jsonresponse.Response.Error.Message : "Not Approved",
						SessionFailedTopic);
				}
			}

			SubscribeForSessionUpdates();
			Events.ListenForResponse<WCSessionRequestResponse>(_handshakeId, HandleSessionResponse);
		}

		private void HandleSessionUpdate(WCSessionData data)
		{
			if (data == null)
			{
				return;
			}

			var wasConnected = SessionConnected;

			//We are connected if we are approved
			SessionConnected = data.approved;

			if (data.chainId != null)
			{
				ChainId = (int)data.chainId;
			}

			if (data.networkId != null)
			{
				NetworkId = (int)data.networkId;
			}

			Accounts = data.accounts;

			switch (wasConnected)
			{
				case false:
					PeerId = data.peerId;
					WalletMetadata = data.peerMeta;
					Events.Trigger(ConnectTopic, data);
					break;
				case true when !SessionConnected:
					HandleSessionDisconnect("Wallet Disconnected");
					break;
				default:
					Events.Trigger("session_update", data);
					break;
			}

			SessionUpdate?.Invoke(this, data);
		}

		private void HandleSessionDisconnect(string msg, string topic = DisconnectTopic)
		{
			SessionConnected = false;
			Disconnected = true;

			Events.Trigger(topic, new ErrorResponse(msg));

			if (TransportConnected)
			{
				DisconnectTransport();
			}

			ActiveTopics.Clear();

			Events.Clear();

			OnSessionDisconnect?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Creates and returns a serializable class that holds all session data required to resume later
		/// </summary>
		/// <returns></returns>
		public SavedSession GetSavedSession()
		{
			if (!SessionConnected || Disconnected)
			{
				return null;
			}

			return new SavedSession(_clientId, _handshakeId, BridgeUrl, Key, KeyRaw, PeerId, NetworkId, Accounts,
				ChainId, DappMetadata, WalletMetadata);
		}
	}
}