namespace MirageSDK.Provider
{
	internal static class MirageSDKAutoCreator
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
			var walletConnectObject = new UnityEngine.GameObject(
				"WalletConnect",
				typeof(MirageSDK.WalletConnectSharp.Unity.WalletConnect)
			);
			UnityEngine.Object.DontDestroyOnLoad(walletConnectObject);
		#endif
		}
	}
}