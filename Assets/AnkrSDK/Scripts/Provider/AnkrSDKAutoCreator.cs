namespace AnkrSDK.Provider
{
	public static class AnkrSDKAutoCreator
	{
		public static void Setup()
		{
		#if (UNITY_WEBGL && !UNITY_EDITOR)
			var walletConnectObject = new UnityEngine.GameObject(
				"WebGLConnect",
				typeof(WebGl.WebGLConnect)
			);
			UnityEngine.Object.DontDestroyOnLoad(walletConnectObject);
		#else
			var walletConnectObject = new UnityEngine.GameObject(
				"WalletConnect",
				typeof(AnkrSDK.WalletConnectSharp.Unity.WalletConnect)
			);
			UnityEngine.Object.DontDestroyOnLoad(walletConnectObject);
		#endif
		}
	}
}