using AnkrSDK.Core;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;

namespace AnkrSDK.Provider
{
	public static class AnkrSDKFactory
	{
		public static IAnkrSDK GetAnkrSDKInstance(string providerURI)
		{
			return CreateAnkrSDKInstance(providerURI);
		}

		public static IAnkrSDK GetAnkrSDKInstance(NetworkName networkName)
		{
			return CreateAnkrSDKInstance(AnkrSDKFactoryHelper.GetAnkrRPCForSelectedNetwork(networkName));
		}

		private static IAnkrSDK CreateAnkrSDKInstance(string providerURI)
		{
		#if (UNITY_WEBGL && !UNITY_EDITOR)
			var web3Provider = new WebGL.WebGLWeb3Provider().CreateWeb3(providerURI);
			var webGlWrapper = new WebGL.WebGLWrapper();
			var contractFunctions = new WebGL.Implementation.ContractFunctionsWebGL(webGlWrapper);
			var eth = new WebGL.Implementation.EthHandlerWebGL(webGlWrapper);
			var disconnectHandler = (IDisconnectHandler)webGlWrapper;
		#else
			var web3Provider = new Mobile.MobileWeb3Provider().CreateWeb3(providerURI);
			var contractFunctions = new Mobile.ContractFunctions(web3Provider);
			var eth = new Mobile.EthHandler(web3Provider);
			var disconnectHandler = new Mobile.MobileDisconnectHandler();
		#endif

			return new AnkrSDKWrapper(web3Provider, contractFunctions, eth, disconnectHandler);
		}
	}
}