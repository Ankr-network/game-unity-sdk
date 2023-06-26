using System;
#if UNITY_WEBGL && !UNITY_EDITOR
using Nethereum.Unity.Metamask;
#endif

namespace MirageSDK.WebGL
{
	public static class WebGLNethereumInteropBridge
	{
		public static bool IsMetamaskAvailable()
		{
			#if UNITY_WEBGL && !UNITY_EDITOR
			return MetamaskWebglInterop.IsMetamaskAvailable();
			#else
			throw new NotImplementedException("WebGLNethereumInterop is not implemented in the Editor");
			#endif
		}

		public static void EthereumInitRpcClientCallback(Action<string> callBackAccountChange,
			Action<string> callBackChainIdChange)
		{
			#if UNITY_WEBGL && !UNITY_EDITOR
			MetamaskWebglInterop.EthereumInitRpcClientCallback(callBackAccountChange, callBackChainIdChange);
			#else
			throw new NotImplementedException("WebGLNethereumInterop is not implemented in the Editor");
			#endif
		}

		public static string GetSelectedAddress()
		{
			#if UNITY_WEBGL && !UNITY_EDITOR
			return MetamaskWebglInterop.GetSelectedAddress();
			#else
			throw new NotImplementedException("WebGLNethereumInterop is not implemented in the Editor");
			#endif
		}

		public static void RequestRpcClientCallback(Action<string> rpcResponse, string rpcRequest)
		{
			#if UNITY_WEBGL && !UNITY_EDITOR
			MetamaskWebglInterop.RequestRpcClientCallback(rpcResponse, rpcRequest);
			#else
			throw new NotImplementedException("WebGLNethereumInterop is not implemented in the Editor");
			#endif
		}

		public static void EnableEthereumRpcClientCallback(
			Action<string> callback,
			Action<string> fallback)
		{
			#if UNITY_WEBGL && !UNITY_EDITOR
			MetamaskWebglInteropExtension.EnableEthereumRpcClientCallback(callback, fallback);
			#else
			throw new NotImplementedException("WebGLNethereumInterop is not implemented in the Editor");
			#endif
		}

		public static void GetChainIdRpcClientCallback(
			Action<string> callback,
			Action<string> fallback)
		{
			#if UNITY_WEBGL && !UNITY_EDITOR
			MetamaskWebglInteropExtension.GetChainIdRpcClientCallback(callback, fallback);
			#else
			throw new NotImplementedException("WebGLNethereumInterop is not implemented in the Editor");
			#endif
		}
	}
}