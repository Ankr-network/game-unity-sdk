#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.EvenHandlers;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Helpers;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Infrastructure;

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

		protected readonly int InstanceId;

		public event WebSocketOpenEventHandler OnOpen;
		public event WebSocketMessageEventHandler OnMessage;
		public event WebSocketErrorEventHandler OnError;
		public event WebSocketCloseEventHandler OnClose;

		public WebGLWebSocket(string url, Dictionary<string, string> headers = null)
		{
			if (!WebSocketNativeBridge.IsInitialized)
			{
				WebSocketNativeBridge.Initialize();
			}

			var instanceId = WebSocketNativeBridge.WebSocketAllocate(url);
			WebSocketNativeBridge.Instances.Add(instanceId, this);

			InstanceId = instanceId;
		}

		public WebGLWebSocket(string url, string subprotocol, Dictionary<string, string> headers = null)
		{
			if (!WebSocketNativeBridge.IsInitialized)
			{
				WebSocketNativeBridge.Initialize();
			}

			var instanceId = WebSocketNativeBridge.WebSocketAllocate(url);
			WebSocketNativeBridge.Instances.Add(instanceId, this);

			WebSocketNativeBridge.WebSocketAddSubProtocol(instanceId, subprotocol);

			InstanceId = instanceId;
		}

		public WebGLWebSocket(string url, List<string> subprotocols, Dictionary<string, string> headers = null)
		{
			if (!WebSocketNativeBridge.IsInitialized)
			{
				WebSocketNativeBridge.Initialize();
			}

			var instanceId = WebSocketNativeBridge.WebSocketAllocate(url);
			WebSocketNativeBridge.Instances.Add(instanceId, this);

			foreach (var subprotocol in subprotocols)
			{
				WebSocketNativeBridge.WebSocketAddSubProtocol(instanceId, subprotocol);
			}

			InstanceId = instanceId;
		}

		~WebGLWebSocket()
		{
			WebSocketNativeBridge.HandleInstanceDestroy(InstanceId);
		}

		public int GetInstanceId()
		{
			return InstanceId;
		}

		public Task Connect()
		{
			var ret = WebSocketConnect(InstanceId);

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

		public void DispatchMessageQueue()
		{
		}

		Task IWebSocket.Close()
		{
			return Close();
		}

		private Task Close(WebSocketCloseCode code = WebSocketCloseCode.Normal, string reason = null)
		{
			var ret = WebSocketClose(InstanceId, (int)code, reason);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return Task.CompletedTask;
		}

		public Task Send(byte[] data)
		{
			var ret = WebSocketSend(InstanceId, data, data.Length);

			if (ret < 0)
			{
				throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);
			}

			return Task.CompletedTask;
		}

		public Task SendText(string message)
		{
			var ret = WebSocketSendText(InstanceId, message);

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
				var state = WebSocketGetState(InstanceId);

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