using MirageSDK.Utils;
using MirageSDK.WalletConnectSharp.Unity;

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
			
			ConnectProvider<MirageSDK.WebGL.WebGLConnect>.GetConnect();
		#else

			var walletConnectUnityAdapter = new UnityEngine.GameObject("WalletConnectAdapter",
				typeof(WalletConnectUnityMonoAdapter));
			UnityEngine.Object.DontDestroyOnLoad(walletConnectUnityAdapter);

			ConnectProvider<WalletConnectSharp.Unity.WalletConnect>.GetConnect();
		#endif
		}
	}
}