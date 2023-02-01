using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.EventHandlers;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WebSocketState = AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data.WebSocketState;

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client.Implementation
{
	public class WebSocket : IWebSocket
	{
		public event WebSocketOpenEventHandler OnOpen;
		public event WebSocketMessageEventHandler OnMessage;
		public event WebSocketErrorEventHandler OnError;
		public event WebSocketCloseEventHandler OnClose;

		private readonly Uri _uri;
		private readonly Dictionary<string, string> _headers;
		private readonly List<string> _subprotocols;
		private readonly Mutex _messageListMutex = new Mutex();
		private readonly List<byte[]> _messageList = new List<byte[]>();

		private ClientWebSocket _socket = new ClientWebSocket();

		private CancellationTokenSource _tokenSource;
		private CancellationToken _cancellationToken;

		private readonly object _lock = new object();

		private bool _isSending;
		private readonly List<ArraySegment<byte>> _sendBytesQueue = new List<ArraySegment<byte>>();
		private readonly List<ArraySegment<byte>> _sendTextQueue = new List<ArraySegment<byte>>();

		public WebSocket(string url, Dictionary<string, string> headers = null)
		{
			_uri = new Uri(url);

			_headers = headers ?? new Dictionary<string, string>();

			_subprotocols = new List<string>();

			var protocol = _uri.Scheme;
			if (!protocol.Equals("ws") && !protocol.Equals("wss"))
			{
				throw new ArgumentException("Unsupported protocol: " + protocol);
			}
		}

		public WebSocket(string url, string subprotocol, Dictionary<string, string> headers = null)
		{
			_uri = new Uri(url);

			_headers = headers ?? new Dictionary<string, string>();

			_subprotocols = new List<string> { subprotocol };

			var protocol = _uri.Scheme;
			if (!protocol.Equals("ws") && !protocol.Equals("wss"))
			{
				throw new ArgumentException("Unsupported protocol: " + protocol);
			}
		}

		public WebSocket(string url, List<string> subprotocols, Dictionary<string, string> headers = null)
		{
			_uri = new Uri(url);

			_headers = headers ?? new Dictionary<string, string>();

			_subprotocols = subprotocols;

			var protocol = _uri.Scheme;
			if (!protocol.Equals("ws") && !protocol.Equals("wss"))
			{
				throw new ArgumentException("Unsupported protocol: " + protocol);
			}
		}

		public void CancelConnection()
		{
			_tokenSource?.Cancel();
		}

		public async UniTask Connect()
		{
			try
			{
				_tokenSource = new CancellationTokenSource();
				_cancellationToken = _tokenSource.Token;

				_socket = new ClientWebSocket();

				foreach (var header in _headers)
				{
					_socket.Options.SetRequestHeader(header.Key, header.Value);
				}

				foreach (var subprotocol in _subprotocols)
				{
					_socket.Options.AddSubProtocol(subprotocol);
				}

				await _socket.ConnectAsync(_uri, _cancellationToken);
				OnOpen?.Invoke();

				await Receive();
			}
			catch (Exception ex)
			{
				OnError?.Invoke(ex.Message);
				OnClose?.Invoke(WebSocketCloseCode.Abnormal);
			}
			finally
			{
				if (_socket != null)
				{
					_tokenSource.Cancel();
					_socket.Dispose();
				}
			}
		}

		public WebSocketState State
		{
			get
			{
				switch (_socket.State)
				{
					case System.Net.WebSockets.WebSocketState.Connecting:
						return WebSocketState.Connecting;

					case System.Net.WebSockets.WebSocketState.Open:
						return WebSocketState.Open;

					case System.Net.WebSockets.WebSocketState.CloseSent:
					case System.Net.WebSockets.WebSocketState.CloseReceived:
						return WebSocketState.Closing;

					case System.Net.WebSockets.WebSocketState.Closed:
						return WebSocketState.Closed;

					default:
						return WebSocketState.Closed;
				}
			}
		}

		public UniTask Send(byte[] bytes)
		{
			// return m_Socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
			return SendMessage(_sendBytesQueue, WebSocketMessageType.Binary, new ArraySegment<byte>(bytes));
		}

		public UniTask SendText(string message)
		{
			var encoded = Encoding.UTF8.GetBytes(message);

			// m_Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
			return SendMessage(_sendTextQueue, WebSocketMessageType.Text,
				new ArraySegment<byte>(encoded, 0, encoded.Length));
		}

		private async UniTask SendMessage(List<ArraySegment<byte>> queue, WebSocketMessageType messageType,
			ArraySegment<byte> buffer)
		{
			// Return control to the calling method immediately.
			// await Task.Yield ();

			// Make sure we have data.
			if (buffer.Count == 0)
			{
				return;
			}

			// The state of the connection is contained in the context Items dictionary.
			bool sending;

			lock (_lock)
			{
				sending = _isSending;

				// If not, we are now.
				if (!_isSending)
				{
					_isSending = true;
				}
			}

			if (!sending)
			{
				// Lock with a timeout, just in case.
				if (!Monitor.TryEnter(_socket, 1000))
				{
					// If we couldn't obtain exclusive access to the socket in one second, something is wrong.
					await _socket.CloseAsync(WebSocketCloseStatus.InternalServerError, string.Empty,
						_cancellationToken);
					return;
				}

				try
				{
					// Send the message synchronously.
					var t = _socket.SendAsync(buffer, messageType, true, _cancellationToken);
					t.Wait(_cancellationToken);
				}
				finally
				{
					Monitor.Exit(_socket);
				}

				// Note that we've finished sending.
				lock (_lock)
				{
					_isSending = false;
				}

				// Handle any queued messages.
				await HandleQueue(queue, messageType);
			}
			else
			{
				// Add the message to the queue.
				lock (_lock)
				{
					queue.Add(buffer);
				}
			}
		}

		private async UniTask HandleQueue(List<ArraySegment<byte>> queue, WebSocketMessageType messageType)
		{
			var buffer = new ArraySegment<byte>();
			lock (_lock)
			{
				// Check for an item in the queue.
				if (queue.Count > 0)
				{
					// Pull it off the top.
					buffer = queue[0];
					queue.RemoveAt(0);
				}
			}

			// Send that message.
			if (buffer.Count > 0)
			{
				await SendMessage(queue, messageType, buffer);
			}
		}

		// simple dispatcher for queued messages.
		public void DispatchMessageQueue()
		{
			// lock mutex, copy queue content and clear the queue.
			_messageListMutex.WaitOne();
			var messageListCopy = new List<byte[]>();
			messageListCopy.AddRange(_messageList);
			_messageList.Clear();
			// release mutex to allow the websocket to add new messages
			_messageListMutex.ReleaseMutex();

			foreach (var bytes in messageListCopy)
			{
				OnMessage?.Invoke(bytes);
			}
		}

		public async UniTask Receive()
		{
			var closeCode = WebSocketCloseCode.Abnormal;
			await new WaitForBackgroundThread();

			var buffer = new ArraySegment<byte>(new byte[8192]);
			try
			{
				while (_socket.State == System.Net.WebSockets.WebSocketState.Open)
				{
					using (var ms = new MemoryStream())
					{
						WebSocketReceiveResult result;
						do
						{
							result = await _socket.ReceiveAsync(buffer, _cancellationToken);
							ms.Write(buffer.Array, buffer.Offset, result.Count);
						} while (!result.EndOfMessage);

						ms.Seek(0, SeekOrigin.Begin);

						if (result.MessageType == WebSocketMessageType.Text)
						{
							_messageListMutex.WaitOne();
							_messageList.Add(ms.ToArray());
							_messageListMutex.ReleaseMutex();

							//using (var reader = new StreamReader(ms, Encoding.UTF8))
							//{
							//	string message = reader.ReadToEnd();
							//	OnMessage?.Invoke(this, new MessageEventArgs(message));
							//}
						}
						else if (result.MessageType == WebSocketMessageType.Binary)
						{
							_messageListMutex.WaitOne();
							_messageList.Add(ms.ToArray());
							_messageListMutex.ReleaseMutex();
						}
						else if (result.MessageType == WebSocketMessageType.Close)
						{
							await Close();
							closeCode = WebSocketHelpers.ParseCloseCodeEnum((int)result.CloseStatus);
							break;
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				_tokenSource.Cancel();
			}
			finally
			{
				await UniTask.SwitchToMainThread();
				OnClose?.Invoke(closeCode);
			}
		}

		public async UniTask Close()
		{
			if (State == WebSocketState.Open)
			{
				await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _cancellationToken);
			}
		}
	}
}