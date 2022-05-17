using AnkrSDK.Core;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Mobile;
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
			var web3Provider = CreateWeb3Provider(providerURI);

		#if (UNITY_WEBGL && !UNITY_EDITOR)
			var webGlWrapper = new WebGL.WebGLWrapper();
			var contractFunctions = new WebGL.Implementation.ContractFunctionsWebGL(webGlWrapper);
			var eth = new AnkrSDK.WebGL.Implementation.EthHandlerWebGL(webGlWrapper);
			var disconnectHandler = (IDisconnectHandler) webGlWrapper;
		#else
			var contractFunctions = new Mobile.ContractFunctions(web3Provider);
			var eth = new Mobile.EthHandler(web3Provider);
			var disconnectHandler = new Mobile.MobileDisconnectHandler();
		#endif

			return new AnkrSDKWrapper(web3Provider, contractFunctions, eth, disconnectHandler);
		}

		private static IWeb3 CreateWeb3Provider(string providerURI)
		{
			IWeb3Provider web3Provider;
		#if (UNITY_WEBGL && !UNITY_EDITOR)
			web3Provider = new WebGLWeb3Provider();
		#else
			web3Provider = new MobileWeb3Provider();
		#endif

			return web3Provider.CreateWeb3(providerURI);
		}
	}
}