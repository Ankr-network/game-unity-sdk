#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Implementation;
using AOT;

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client
{
	/// <summary>
	/// Class providing static access methods to work with JSLIB WebSocket or WebSocketSharp interface
	/// </summary>
	public static class WebSocketNativeBridge
	{
		public static readonly Dictionary<int, WebGLWebSocket> Instances = new Dictionary<int, WebGLWebSocket>();

		public delegate void OnOpenCallback(int instanceId);

		public delegate void OnMessageCallback(int instanceId, IntPtr msgPtr, int msgSize);

		public delegate void OnErrorCallback(int instanceId, IntPtr errorPtr);

		public delegate void OnCloseCallback(int instanceId, int closeCode);

		/* WebSocket JSLIB callback setters and other functions */
		[DllImport("__Internal")]
		public static extern int WebSocketAllocate(string url);

		[DllImport("__Internal")]
		public static extern int WebSocketAddSubProtocol(int instanceId, string subprotocol);

		[DllImport("__Internal")]
		public static extern void WebSocketFree(int instanceId);

		[DllImport("__Internal")]
		public static extern void WebSocketSetOnOpen(OnOpenCallback callback);

		[DllImport("__Internal")]
		public static extern void WebSocketSetOnMessage(OnMessageCallback callback);

		[DllImport("__Internal")]
		public static extern void WebSocketSetOnError(OnErrorCallback callback);

		[DllImport("__Internal")]
		public static extern void WebSocketSetOnClose(OnCloseCallback callback);

		/* If callbacks was initialized and set */
		public static bool IsInitialized;

		/*
		 * Initialize WebSocket callbacks to JSLIB
		 */
		public static void Initialize()
		{
			WebSocketSetOnOpen(DelegateOnOpenEvent);
			WebSocketSetOnMessage(DelegateOnMessageEvent);
			WebSocketSetOnError(DelegateOnErrorEvent);
			WebSocketSetOnClose(DelegateOnCloseEvent);

			IsInitialized = true;
		}

		/// <summary>
		/// Called when instance is destroyed (by destructor)
		/// Method removes instance from map and free it in JSLIB implementation
		/// </summary>
		/// <param name="instanceId">Instance identifier.</param>
		public static void HandleInstanceDestroy(int instanceId)
		{
			Instances.Remove(instanceId);
			WebSocketFree(instanceId);
		}
		[MonoPInvokeCallback(typeof(OnOpenCallback))]
		public static void DelegateOnOpenEvent(int instanceId)
		{
			if (Instances.TryGetValue(instanceId, out var instanceRef))
			{
				instanceRef.DelegateOnOpenEvent();
			}
		}
		[MonoPInvokeCallback(typeof(OnMessageCallback))]
		public static void DelegateOnMessageEvent(int instanceId, IntPtr msgPtr, int msgSize)
		{
			if (Instances.TryGetValue(instanceId, out var instanceRef))
			{
				var msg = new byte[msgSize];
				Marshal.Copy(msgPtr, msg, 0, msgSize);

				instanceRef.DelegateOnMessageEvent(msg);
			}
		}
		[MonoPInvokeCallback(typeof(OnErrorCallback))]
		public static void DelegateOnErrorEvent(int instanceId, IntPtr errorPtr)
		{
			if (Instances.TryGetValue(instanceId, out var instanceRef))
			{
				var errorMsg = Marshal.PtrToStringAuto(errorPtr);
				instanceRef.DelegateOnErrorEvent(errorMsg);
			}
		}
		[MonoPInvokeCallback(typeof(OnCloseCallback))]
		public static void DelegateOnCloseEvent(int instanceId, int closeCode)
		{
			if (Instances.TryGetValue(instanceId, out var instanceRef))
			{
				instanceRef.DelegateOnCloseEvent(closeCode);
			}
		}
	}
}
#endif