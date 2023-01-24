using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Unity;
using UnityEngine;

namespace AnkrSDK.Provider
{
	internal static class AnkrSDKAutoCreator
	{
		internal static void Setup()
		{
		#if (UNITY_WEBGL && !UNITY_EDITOR)
			var walletConnectObject = new UnityEngine.GameObject(
				"WebGLConnect",
				typeof(WebGL.WebGLConnect)
			);
			UnityEngine.Object.DontDestroyOnLoad(walletConnectObject);
		#else

			var walletConnectUnityAdapter = new UnityEngine.GameObject("WalletConnectAdapter",
				typeof(WalletConnectUnityMonoAdapter));

			WalletConnectProvider.GetWalletConnect();
		#endif
		}
	}
}