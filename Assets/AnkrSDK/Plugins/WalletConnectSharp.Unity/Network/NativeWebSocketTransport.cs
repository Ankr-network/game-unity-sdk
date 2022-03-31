using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Events;
using AnkrSDK.WalletConnectSharp.Core.Events.Model;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Network;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Unity.Network
{
	public class NativeWebSocketTransport : MonoBehaviour, ITransport
	{
		private bool opened = false;
		private bool closed = false;

		private WebSocket nextClient;
		private WebSocket client;
		private EventDelegator _eventDelegator;
		private bool wasPaused;
		private List<string> subscribedTopics = new List<string>();
		private Queue<NetworkMessage> _queuedMessages = new Queue<NetworkMessage>();

		public bool Connected => client?.State == WebSocketState.Open && opened;

		public void AttachEventDelegator(EventDelegator eventDelegator)
		{
			_eventDelegator = eventDelegator;
		}

		public void Dispose()
		{
			client?.CancelConnection();
		}

		public event EventHandler<MessageReceivedEventArgs> MessageReceived;
		public event EventHandler<MessageReceivedEventArgs> OpenReceived;
		public event EventHandler<MessageReceivedEventArgs> Closed;

		public string URL { get; private set; }

		public async Task Open(string url, bool clearSubscriptions = true)
		{
			if (URL != url || clearSubscriptions)
			{
				ClearSubscriptions();
			}

			URL = url;

			await _socketOpen();
		}

		private async Task _socketOpen()
		{
			Debug.Log("[WebSocket] Trying to open socket");
			if (nextClient != null)
			{
				if (nextClient.State == WebSocketState.Closed)
				{
					Debug.LogError("[WebSocket] Socket was closed but not cleared");
					nextClient = null;
				}
				else
				{
					Debug.Log(
						$"[WebSocket] Will not try to open socket because it is already in state: {nextClient.State}");
					return;
				}
			}

			var url = URL;
			if (url.StartsWith("https"))
			{
				url = url.Replace("https", "wss");
			}
			else if (url.StartsWith("http"))
			{
				url = url.Replace("http", "ws");
			}

			nextClient = new WebSocket(url);

			var eventCompleted = new TaskCompletionSource<bool>(TaskCreationOptions.None);

			nextClient.OnOpen += () =>
			{
				CompleteOpen();

				// subscribe now
				OpenReceived?.Invoke(this, null);

				Debug.Log("[WebSocket] Opened " + url);

				eventCompleted.SetResult(true);
			};

			nextClient.OnMessage += OnMessageReceived;
			nextClient.OnClose += ClientTryReconnect;
			nextClient.OnError += e =>
			{
				Debug.Log("[WebSocket] OnError " + e);
				HandleError(new Exception(e));
			};

			nextClient.Connect().ContinueWith(t => HandleError(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

			Debug.Log("[WebSocket] Waiting for Open " + url);
			await eventCompleted.Task;
			Debug.Log("[WebSocket] Open Completed");
		}

		private void HandleError(Exception e)
		{
			Debug.LogError(e);
		}

		private async void CompleteOpen()
		{
			await Close();
			client = nextClient;
			nextClient = null;
			QueueSubscriptions();
			opened = true;
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
			foreach (var topic in subscribedTopics)
			{
				_queuedMessages.Enqueue(GenerateSubscribeMessage(topic));
			}

			Debug.Log("[WebSocket] Queued " + subscribedTopics.Count + " subscriptions");
		}

		private async void ClientTryReconnect(WebSocketCloseCode closeCode)
		{
			if (wasPaused)
			{
				Debug.Log("[WebSocket] Application paused, retry attempt aborted");
				return;
			}

			nextClient = null;

			if (closeCode == WebSocketCloseCode.Abnormal)
			{
				const float reconnectDelay = 2f;

				Debug.LogError($"Abnormal close detected. Waiting for {reconnectDelay}s before reconnect");

				await UniTask.Delay(TimeSpan.FromSeconds(reconnectDelay));
			}

			await _socketOpen();
		}

		public void CancelConnection()
		{
			client.CancelConnection();
		}

		private async void OnMessageReceived(byte[] bytes)
		{
			var json = System.Text.Encoding.UTF8.GetString(bytes);

			try
			{
				var msg = JsonConvert.DeserializeObject<NetworkMessage>(json);


				await SendMessage(new NetworkMessage()
				{
					Payload = "",
					Type = "ack",
					Silent = true,
					Topic = msg.Topic
				});

				MessageReceived?.Invoke(this, new MessageReceivedEventArgs(msg, this));
			}
			catch (Exception e)
			{
				Debug.Log("[WebSocket] Exception " + e.Message);
			}
		}

		private void Update()
		{
		#if !UNITY_WEBGL || UNITY_EDITOR
			if (client?.State == WebSocketState.Open)
			{
				client.DispatchMessageQueue();
			}
		#endif
		}

		public async Task Close()
		{
			Debug.Log("Closing Websocket");
			try
			{
				if (client != null)
				{
					opened = false;
					client.OnClose -= ClientTryReconnect;
					await client.Close();
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

		public async Task SendMessage(NetworkMessage message)
		{
			if (!Connected)
			{
				_queuedMessages.Enqueue(message);
				await _socketOpen();
			}
			else
			{
				var finalJson = JsonConvert.SerializeObject(message);

				await client.SendText(finalJson);
			}
		}

		public async Task Subscribe(string topic)
		{
			Debug.Log("[WebSocket] Subscribe to " + topic);

			var msg = GenerateSubscribeMessage(topic);

			await SendMessage(msg);

			if (!subscribedTopics.Contains(topic))
			{
				subscribedTopics.Add(topic);
			}

			opened = true;
		}

		private NetworkMessage GenerateSubscribeMessage(string topic)
		{
			return new NetworkMessage()
			{
				Payload = "",
				Type = "sub",
				Silent = true,
				Topic = topic
			};
		}

		public async Task Subscribe<T>(string topic, EventHandler<JsonRpcResponseEvent<T>> callback)
			where T : JsonRpcResponse
		{
			await Subscribe(topic);

			_eventDelegator.ListenFor(topic, callback);
		}

		public async Task Subscribe<T>(string topic, EventHandler<JsonRpcRequestEvent<T>> callback)
			where T : JsonRpcRequest
		{
			await Subscribe(topic);

			_eventDelegator.ListenFor(topic, callback);
		}

		public void ClearSubscriptions()
		{
			if (_eventDelegator != null)
			{
				foreach (var subscribedTopic in subscribedTopics)
				{
					_eventDelegator.UnsubscribeProvider(subscribedTopic);
				}
			}

			subscribedTopics.Clear();
			_queuedMessages.Clear();
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			ProcessApplicationPause(pauseStatus).Forget();
		}

		private async UniTask ProcessApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				Debug.Log("[WebSocket] Pausing");
				wasPaused = true;
				await Close();
			}
			else if (wasPaused)
			{
				wasPaused = false;
				Debug.Log("[WebSocket] Resuming");
				await Open(URL, false);

				foreach (var topic in subscribedTopics)
				{
					await Subscribe(topic);
				}
			}
		}
	}
}