using System;
using AnkrSDK.Core;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using Nethereum.Web3;

namespace AnkrSDK.Provider
{
	public static class AnkrSDKFactory
	{
		public static IAnkrSDK GetAnkrSDKInstance(string providerURI)
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
		
		public static IAnkrSDK GetAnkrSDKInstance(NetworkName networkName)
		{
			var web3Provider = CreateWeb3Provider(GetAnkrRPCForSelectedNetwork(networkName));

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
		#if (UNITY_WEBGL && !UNITY_EDITOR)
			return WebGL.WebGLWeb3Provider.CreateWeb3Provider(providerURI);
		#else
			return Mobile.MobileWeb3Provider.CreateWeb3Provider(providerURI);
		#endif
		}

		private static string GetAnkrRPCForSelectedNetwork(NetworkName networkName)
		{
			switch (networkName)
			{
				case NetworkName.Ethereum:
					return "https://rpc.ankr.com/eth";
				case NetworkName.Ethereum_Rinkeby_TestNet:
					return "https://rpc.ankr.com/eth_rinkeby";
				case NetworkName.Ethereum_Goerli_TestNet:
					return "https://rpc.ankr.com/eth_goerli";
				case NetworkName.Ethereum_Ropsten_TestNet:
					return "https://rpc.ankr.com/eth_ropsten";
				case NetworkName.BinanceSmartChain:
					return "https://rpc.ankr.com/bsc";
				case NetworkName.BinanceSmartChain_TestNet:
					default: throw new NotSupportedException();
			}
		}
	}
}