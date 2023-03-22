using System;
using System.Collections.Generic;
using AnkrSDK.WalletConnect.VersionShared.Utils;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Network;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Exceptions;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Implementation;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Unity.Network
{
	public class NativeWebSocketTransport : ITransport
	{
		private readonly List<string> _subscribedTopics = new List<string>();
		private readonly Queue<NetworkMessage> _queuedMessages = new Queue<NetworkMessage>();

		private bool _opened;
		private bool _wasPaused;

		private WebSocket _nextClient;
		private WebSocket _client;

		public bool Connected => _client?.State == WebSocketState.Open && _opened;
		public string URL { get; private set; }

		public event EventHandler<MessageReceivedEventArgs> MessageReceived;
		public event EventHandler<MessageReceivedEventArgs> OpenReceived;
		public event EventHandler<MessageReceivedEventArgs> Closed;

		public void Update()
		{
		#if !UNITY_WEBGL || UNITY_EDITOR
			if (_client?.State == WebSocketState.Open)
			{
				_client.DispatchMessageQueue();
			}
		#endif
		}

		public async UniTask OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				Debug.Log("[WebSocket] Pausing");
				_wasPaused = true;
				await Close();
			}
			else if (_wasPaused)
			{
				_wasPaused = false;
				Debug.Log("[WebSocket] Resuming");
				await Open(URL, false);

				foreach (var topic in _subscribedTopics)
				{
					await Subscribe(topic);
				}
			}
		}

		public void Dispose()
		{
			_client?.CancelConnection();
		}

		public UniTask Open(string url, bool clearSubscriptions = true)
		{
			if (URL != url || clearSubscriptions)
			{
				ClearSubscriptions();
			}

			URL = url;

			return OpenSocket();
		}

		public async UniTask Close()
		{
			Debug.Log("Closing Websocket");
			try
			{
				if (_client != null)
				{
					_opened = false;
					_client.OnClose -= ClientTryReconnect;
					await _client.Close();
				}
			}
			catch (WebSocketInvalidStateException e)
			{
				if (e.Message.Contains("WebSocket is not connected"))
				{
					Debug.LogWarning("Tried to close a websocket when it's already closed");
				}
				else
				{
					throw;
				}
			}
			finally
			{
				Closed?.Invoke(this, null);
			}
		}

		public async UniTask SendMessage(NetworkMessage message)
		{
			if (!Connected)
			{
				_queuedMessages.Enqueue(message);
				await OpenSocket();
			}
			else
			{
				var finalJson = JsonConvert.SerializeObject(message);

				await _client.SendText(finalJson);
			}
		}

		public async UniTask Subscribe(string topic)
		{
			Debug.Log("[WebSocket] Subscribe to " + topic);

			var msg = NetworkMessageHelper.GenerateSubscribeMessage(topic);

			await SendMessage(msg);

			if (!_subscribedTopics.Contains(topic))
			{
				_subscribedTopics.Add(topic);
			}

			_opened = true;
		}

		public void ClearSubscriptions()
		{
			_subscribedTopics.Clear();
			_queuedMessages.Clear();
		}

		private async UniTask OpenSocket()
		{
			Debug.Log("[WebSocket] Trying to open socket");
			if (_nextClient != null)
			{
				if (_nextClient.State == WebSocketState.Closed)
				{
					Debug.LogError("[WebSocket] Socket was closed but not cleared");
					_nextClient = null;
				}
				else
				{
					Debug.Log(
						$"[WebSocket] Will not try to open socket because it is already in state: {_nextClient.State}");
					return;
				}
			}

			var url = WSUrlFormatter.GetReadyToUseURL(URL);

			var eventCompleted = ConfigureNextClient(url, out _nextClient);

			StartClientConnect().Forget();

			Debug.Log("[WebSocket] Waiting for Open " + url);
			await eventCompleted.Task;
			Debug.Log("[WebSocket] Open Completed");
		}

		private UniTaskCompletionSource<bool> ConfigureNextClient(string url, out WebSocket nextClient)
		{
			nextClient = new WebSocket(url);

			var eventCompleted = new UniTaskCompletionSource<bool>();

			void OnNextClientOpen()
			{
				CompleteOpen();

				// subscribe now
				OpenReceived?.Invoke(this, null);

				Debug.Log("[WebSocket] Opened " + url);

				eventCompleted.TrySetResult(true);
			}

			nextClient.OnOpen += OnNextClientOpen;

			nextClient.OnMessage += OnMessageReceived;
			nextClient.OnClose += ClientTryReconnect;
			nextClient.OnError += e => { HandleError(new Exception(e)); };
			return eventCompleted;
		}

		private async UniTaskVoid StartClientConnect()
		{
			var connectTask = _nextClient.Connect().AsTask();
			await connectTask;
			if (connectTask.IsFaulted)
			{
				HandleError(connectTask.Exception);
			}
		}

		private static void HandleError(Exception e)
		{
			Debug.LogException(e);
		}

		private async void CompleteOpen()
		{
			Debug.Log("Closing OLD client");
			await Close();
			_client = _nextClient;
			_nextClient = null;
			QueueSubscriptions();
			_opened = true;
			FlushQueue();
		}

		private async void FlushQueue()
		{
			Debug.Log("[WebSocket] Flushing Queue. Count: " + _queuedMessages.Count);
			while (_queuedMessages.Count > 0)
			{
				var msg = _queuedMessages.Dequeue();
				await SendMessage(msg);
			}

			Debug.Log("[WebSocket] Queue Flushed");
		}

		private void QueueSubscriptions()
		{
			foreach (var topic in _subscribedTopics)
			{
				_queuedMessages.Enqueue(NetworkMessageHelper.GenerateSubscribeMessage(topic));
			}

			Debug.Log("[WebSocket] Queued " + _subscribedTopics.Count + " subscriptions");
		}

		private async void ClientTryReconnect(WebSocketCloseCode closeCode)
		{
			if (_wasPaused)
			{
				Debug.Log("[WebSocket] Application paused, retry attempt aborted");
				return;
			}

			_nextClient = null;

			if (closeCode == WebSocketCloseCode.Abnormal)
			{
				const float reconnectDelay = 2f;

				Debug.LogError($"Abnormal close detected. Waiting for {reconnectDelay}s before reconnect");

				await UniTask.Delay(TimeSpan.FromSeconds(reconnectDelay));
			}

			await OpenSocket();
		}

		private async void OnMessageReceived(byte[] bytes)
		{
			var json = System.Text.Encoding.UTF8.GetString(bytes);

			try
			{
				var msg = JsonConvert.DeserializeObject<NetworkMessage>(json);

				await SendMessage(NetworkMessageHelper.GenerateAckMessage(msg.Topic));

				MessageReceived?.Invoke(this, new MessageReceivedEventArgs(msg, this));
			}
			catch (Exception e)
			{
				Debug.Log("[WebSocket] Exception " + e.Message);
			}
		}
	}
}