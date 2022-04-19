using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using AOT;
using System.Runtime.InteropServices;
#endif

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client
{
#if UNITY_WEBGL && !UNITY_EDITOR

	/// <summary>
	/// WebSocket class bound to JSLIB.
	/// </summary>
	public class WebSocket : IWebSocket
	{
		/* WebSocket JSLIB functions */
		[DllImport("__Internal")]
		public static extern int WebSocketConnect(int instanceId);

		[DllImport("__Internal")]
		public static extern int WebSocketClose(int instanceId, int code, string reason);

		[DllImport("__Internal")]
		public static extern int WebSocketSend(int instanceId, byte[] dataPtr, int dataLength);

		[DllImport("__Internal")]
		public static extern int WebSocketSendText(int instanceId, string message);

		[DllImport("__Internal")]
		public static extern int WebSocketGetState(int instanceId);

		protected int instanceId;

		public event WebSocketOpenEventHandler OnOpen;
		public event WebSocketMessageEventHandler OnMessage;
		public event WebSocketErrorEventHandler OnError;
		public event WebSocketCloseEventHandler OnClose;

		public WebSocket(string url)
		{
			if (!WebSocketFactory.isInitialized)
			{
				WebSocketFactory.Initialize();
			}

			int allocatedSocketId = WebSocketFactory.WebSocketAllocate(url);
			WebSocketFactory.instances.Add(allocatedSocketId, this);

			instanceId = allocatedSocketId;
		}

		public WebSocket(string url, string subprotocol)
		{
			if (!WebSocketFactory.isInitialized)
			{
				WebSocketFactory.Initialize();
			}

			int allocatedSocketId = WebSocketFactory.WebSocketAllocate(url);
			WebSocketFactory.instances.Add(allocatedSocketId, this);

			WebSocketFactory.WebSocketAddSubProtocol(allocatedSocketId, subprotocol);

			instanceId = allocatedSocketId;
		}

		public WebSocket(string url, List<string> subprotocols)
		{
			if (!WebSocketFactory.isInitialized)
			{
				WebSocketFactory.Initialize();
			}

			int allocatedSocketId = WebSocketFactory.WebSocketAllocate(url);
			WebSocketFactory.instances.Add(instanceId, this);

			foreach (var subprotocol in subprotocols)
			{
				WebSocketFactory.WebSocketAddSubProtocol(instanceId, subprotocol);
			}

			instanceId = allocatedSocketId;
		}

		~WebSocket()
		{
			WebSocketFactory.HandleInstanceDestroy(instanceId);
		}

		public int GetInstanceId()
		{
			return instanceId;
		}

		public Task Connect()
		{
			var ret = WebSocketConnect(instanceId);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return Task.CompletedTask;
		}

		public void CancelConnection()
		{
			if (State == WebSocketState.Open)
			{
				Close(WebSocketCloseCode.Abnormal);
			}
		}

		public Task Close(WebSocketCloseCode code = WebSocketCloseCode.Normal, string reason = null)
		{
			var ret = WebSocketClose(instanceId, (int)code, reason);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return Task.CompletedTask;
		}

		public Task Send(byte[] data)
		{
			var ret = WebSocketSend(instanceId, data, data.Length);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return Task.CompletedTask;
		}

		public Task SendText(string message)
		{
			var ret = WebSocketSendText(instanceId, message);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return Task.CompletedTask;
		}

		public WebSocketState State
		{
			get
			{
				var state = WebSocketGetState(instanceId);

				if (state < 0)
				{
					throw WebSocketHelpers.GetErrorMessageFromCode(state, null);
				}

				switch (state)
				{
					case 0:
						return WebSocketState.Connecting;

					case 1:
						return WebSocketState.Open;

					case 2:
						return WebSocketState.Closing;

					case 3:
						return WebSocketState.Closed;

					default:
						return WebSocketState.Closed;
				}
			}
		}

		public void DispatchMessageQueue()
		{
		}

		public void DelegateOnOpenEvent()
		{
			OnOpen?.Invoke();
		}

		public void DelegateOnMessageEvent(byte[] data)
		{
			OnMessage?.Invoke(data);
		}

		public void DelegateOnErrorEvent(string errorMsg)
		{
			OnError?.Invoke(errorMsg);
		}

		public void DelegateOnCloseEvent(int closeCode)
		{
			OnClose?.Invoke(WebSocketHelpers.ParseCloseCodeEnum(closeCode));
		}
	}

#else
    public class WebSocket : IWebSocket
    {
        public event WebSocketOpenEventHandler OnOpen;
        public event WebSocketMessageEventHandler OnMessage;
        public event WebSocketErrorEventHandler OnError;
        public event WebSocketCloseEventHandler OnClose;

        private readonly Uri _uri;
        private readonly Dictionary<string, string> _headers;
        private readonly List<string> _subprotocols;
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

            _subprotocols = new List<string> {subprotocol};

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

        public async Task Connect()
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

        public Task Send(byte[] bytes)
        {
            // return m_Socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
            return SendMessage(_sendBytesQueue, WebSocketMessageType.Binary, new ArraySegment<byte>(bytes));
        }

        public Task SendText(string message)
        {
            var encoded = Encoding.UTF8.GetBytes(message);

            // m_Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            return SendMessage(_sendTextQueue, WebSocketMessageType.Text,
                new ArraySegment<byte>(encoded, 0, encoded.Length));
        }

        private async Task SendMessage(List<ArraySegment<byte>> queue, WebSocketMessageType messageType,
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

        private async Task HandleQueue(List<ArraySegment<byte>> queue, WebSocketMessageType messageType)
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

        private Mutex m_MessageListMutex = new Mutex();
        private List<byte[]> m_MessageList = new List<byte[]>();

        // simple dispatcher for queued messages.
        public void DispatchMessageQueue()
        {
            // lock mutex, copy queue content and clear the queue.
            m_MessageListMutex.WaitOne();
            var messageListCopy = new List<byte[]>();
            messageListCopy.AddRange(m_MessageList);
            m_MessageList.Clear();
            // release mutex to allow the websocket to add new messages
            m_MessageListMutex.ReleaseMutex();

            foreach (var bytes in messageListCopy)
            {
                OnMessage?.Invoke(bytes);
            }
        }

        public async Task Receive()
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
                            m_MessageListMutex.WaitOne();
                            m_MessageList.Add(ms.ToArray());
                            m_MessageListMutex.ReleaseMutex();

                            //using (var reader = new StreamReader(ms, Encoding.UTF8))
                            //{
                            //	string message = reader.ReadToEnd();
                            //	OnMessage?.Invoke(this, new MessageEventArgs(message));
                            //}
                        }
                        else if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            m_MessageListMutex.WaitOne();
                            m_MessageList.Add(ms.ToArray());
                            m_MessageListMutex.ReleaseMutex();
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await Close();
                            closeCode = WebSocketHelpers.ParseCloseCodeEnum((int) result.CloseStatus);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                _tokenSource.Cancel();
            }
            finally
            {
                await UniTask.SwitchToMainThread();
                OnClose?.Invoke(closeCode);
            }
        }

        public async Task Close()
        {
            if (State == WebSocketState.Open)
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _cancellationToken);
            }
        }
    }
#endif
}