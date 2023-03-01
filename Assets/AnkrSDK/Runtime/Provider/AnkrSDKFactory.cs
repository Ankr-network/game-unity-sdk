using AnkrSDK.Core;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Utils;

namespace AnkrSDK.Provider
{
	public static class AnkrSDKFactory
	{
		/// <summary>
		/// Use this method to get ANKR SDK Instance and start interacting with the blockchain.
		/// </summary>
		/// <param name="providerURI">Your selected provider URI</param>
		/// <param name="autoSetup">Automatically setup connection object?
		/// <para>If true - creates required object on scene.</para>
		/// <para>If false - looks for it in the scene</para></param>
		/// <returns>Ready to work IAnkrSDK instance</returns>
		public static IAnkrSDK GetAnkrSDKInstance(string providerURI, bool autoSetup = false)
		{
			return CreateAnkrSDKInstance(providerURI, autoSetup);
		}

		/// <summary>
		/// Use this method to get ANKR SDK Instance and start interacting with the blockchain.
		/// </summary>
		/// <param name="networkName">Network to work withI</param>
		/// <param name="autoSetup">Automatically setup connection object?
		/// <para>If true - creates required object on scene.</para>
		/// <para>If false - looks for it in the scene</para></param>
		/// <returns>Ready to work IAnkrSDK instance</returns>
		public static IAnkrSDK GetAnkrSDKInstance(NetworkName networkName, bool autoSetup = false)
		{
			return CreateAnkrSDKInstance(AnkrSDKNetworkUtils.GetAnkrRPCForSelectedNetwork(networkName), autoSetup);
		}

		private static IAnkrSDK CreateAnkrSDKInstance(string providerURI, bool autoSetup)
		{
			if (autoSetup)
			{
				AnkrSDKAutoCreator.Setup();
			}

			ISilentSigningHandler silentSigningHandler = null;
		#if (UNITY_WEBGL && !UNITY_EDITOR)
			var webGlWrapper = Utils.ConnectProvider<AnkrSDK.WebGL.WebGLConnect>.GetConnect().SessionWrapper;
			var contractFunctions = new WebGL.Implementation.ContractFunctionsWebGL(webGlWrapper);
			var eth = new WebGL.Implementation.EthHandlerWebGL(webGlWrapper);
			var walletHandler = (IWalletHandler)webGlWrapper;
			var networkHandler = new WebGL.Implementation.AnkrNetworkWebGLHelper(webGlWrapper);
		#else
			silentSigningHandler = new AnkrSDK.SilentSigning.Implementation.SilentSigningProtocol();
			var web3Provider = new Mobile.MobileWeb3Provider().CreateWeb3(providerURI);
			var contractFunctions = new Mobile.ContractFunctions(web3Provider);
			var eth = new Mobile.EthHandler(web3Provider, silentSigningHandler);
			var walletHandler = new Mobile.MobileWalletHandler();
			var networkHandler = new Mobile.AnkrNetworkHelper();
		#endif

			return new AnkrSDKWrapper(
				contractFunctions,
				eth,
				walletHandler,
				networkHandler,
				silentSigningHandler
			);
		}
	}
}