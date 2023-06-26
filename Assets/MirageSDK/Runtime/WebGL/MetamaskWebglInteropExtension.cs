using System;
using System.Runtime.InteropServices;

namespace MirageSDK.WebGL
{
	public class MetamaskWebglInteropExtension
	{
		[DllImport("__Internal")]
		public static extern void EnableEthereumRpcClientCallback(
			Action<string> callback,
			Action<string> fallback);

		[DllImport("__Internal")]
		public static extern void GetChainIdRpcClientCallback(
			Action<string> callback,
			Action<string> fallback);
	}
}