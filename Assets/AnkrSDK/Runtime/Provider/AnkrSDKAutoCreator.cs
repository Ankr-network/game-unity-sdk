
using AnkrSDK.Plugins.WalletConnectSharp.Unity;
using AnkrSDK.Utils;

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
			
			ConnectProvider<AnkrSDK.WebGL.WebGLConnect>.GetConnect();
		#else

			var walletConnectUnityAdapter = new UnityEngine.GameObject("WalletConnectAdapter",
				typeof(WalletConnectUnityMonoAdapter));
			UnityEngine.Object.DontDestroyOnLoad(walletConnectUnityAdapter);

			ConnectProvider<WalletConnect>.GetConnect();
		#endif
		}
	}
}