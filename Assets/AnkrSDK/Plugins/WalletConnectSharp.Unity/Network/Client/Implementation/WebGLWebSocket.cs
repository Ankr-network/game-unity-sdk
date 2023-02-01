#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.EventHandlers;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Infrastructure;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client.Implementation
{
	/// <summary>
	/// WebSocket class bound to JSLIB.
	/// </summary>
	public class WebGLWebSocket : IWebSocket
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

		private readonly int _instanceId;

		public event WebSocketOpenEventHandler OnOpen;
		public event WebSocketMessageEventHandler OnMessage;
		public event WebSocketErrorEventHandler OnError;
		public event WebSocketCloseEventHandler OnClose;

		public WebGLWebSocket(string url)
		{
			if (!WebGLWebSocketNativeBridge.isInitialized)
			{
				WebGLWebSocketNativeBridge.Initialize();
			}

			int allocatedSocketId = WebGLWebSocketNativeBridge.WebSocketAllocate(url);
			WebGLWebSocketNativeBridge.Instances.Add(allocatedSocketId, this);

			_instanceId = allocatedSocketId;
		}

		public WebGLWebSocket(string url, string subprotocol)
		{
			if (!WebGLWebSocketNativeBridge.isInitialized)
			{
				WebGLWebSocketNativeBridge.Initialize();
			}

			int allocatedSocketId = WebGLWebSocketNativeBridge.WebSocketAllocate(url);
			WebGLWebSocketNativeBridge.Instances.Add(allocatedSocketId, this);

			WebGLWebSocketNativeBridge.WebSocketAddSubProtocol(allocatedSocketId, subprotocol);

			_instanceId = allocatedSocketId;
		}

		public WebGLWebSocket(string url, List<string> subprotocols)
		{
			if (!WebGLWebSocketNativeBridge.isInitialized)
			{
				WebGLWebSocketNativeBridge.Initialize();
			}

			int allocatedSocketId = WebGLWebSocketNativeBridge.WebSocketAllocate(url);
			WebGLWebSocketNativeBridge.Instances.Add(_instanceId, this);

			foreach (var subprotocol in subprotocols)
			{
				WebGLWebSocketNativeBridge.WebSocketAddSubProtocol(_instanceId, subprotocol);
			}

			_instanceId = allocatedSocketId;
		}

		~WebGLWebSocket()
		{
			WebGLWebSocketNativeBridge.HandleInstanceDestroy(_instanceId);
		}

		public int GetInstanceId()
		{
			return _instanceId;
		}

		public UniTask Connect()
		{
			var ret = WebSocketConnect(_instanceId);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return UniTask.CompletedTask;
		}

		public UniTask Close()
		{
			if (State == WebSocketState.Open)
			{
				return Close(WebSocketCloseCode.Abnormal);
			}

			return UniTask.CompletedTask;
		}

		public UniTask Close(WebSocketCloseCode code = WebSocketCloseCode.Normal, string reason = null)
		{
			var ret = WebSocketClose(_instanceId, (int)code, reason);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return UniTask.CompletedTask;
		}

		public UniTask Send(byte[] data)
		{
			var ret = WebSocketSend(_instanceId, data, data.Length);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return UniTask.CompletedTask;
		}

		public UniTask SendText(string message)
		{
			var ret = WebSocketSendText(_instanceId, message);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return UniTask.CompletedTask;
		}

		public WebSocketState State
		{
			get
			{
				var state = WebSocketGetState(_instanceId);

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
}
#endif