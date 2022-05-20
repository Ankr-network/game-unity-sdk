using AnkrSDK.Core;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using Nethereum.Web3;

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
			var webGlWrapper = new WebGL.WebGLWrapper();
			var contractFunctions = new WebGL.Implementation.ContractFunctionsWebGL(webGlWrapper);
			var eth = new WebGL.Implementation.EthHandlerWebGL(webGlWrapper);
			var disconnectHandler = (IDisconnectHandler)webGlWrapper;
			var networkHandler = new AnkrNetworkWebGLHelper(webGlWrapper);
		#else
			var web3Provider = new Mobile.MobileWeb3Provider().CreateWeb3(providerURI);
			var contractFunctions = new Mobile.ContractFunctions(web3Provider);
			var eth = new Mobile.EthHandler(web3Provider);
			var disconnectHandler = new Mobile.MobileDisconnectHandler();
			var networkHandler = new AnkrNetworkHelper();
		#endif

			return new AnkrSDKWrapper(contractFunctions, eth, disconnectHandler, networkHandler);
		}
	}
}