using MirageSDK.Core;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.Utils;

namespace MirageSDK.Provider
{
	public static class MirageSDKFactory
	{
		/// <summary>
		/// Use this method to get ANKR SDK Instance and start interacting with the blockchain.
		/// </summary>
		/// <param name="providerURI">Your selected provider URI</param>
		/// <param name="autoSetup">Automatically setup connection object?
		/// <para>If true - creates required object on scene.</para>
		/// <para>If false - looks for it in the scene</para></param>
		/// <returns>Ready to work IMirageSDK instance</returns>
		public static IMirageSDK GetMirageSDKInstance(string providerURI, bool autoSetup = false)
		{
			return CreateMirageSDKInstance(providerURI, autoSetup);
		}

		/// <summary>
		/// Use this method to get ANKR SDK Instance and start interacting with the blockchain.
		/// </summary>
		/// <param name="networkName">Network to work withI</param>
		/// <param name="autoSetup">Automatically setup connection object?
		/// <para>If true - creates required object on scene.</para>
		/// <para>If false - looks for it in the scene</para></param>
		/// <returns>Ready to work IMirageSDK instance</returns>
		public static IMirageSDK GetMirageSDKInstance(NetworkName networkName, bool autoSetup = false)
		{
			return CreateMirageSDKInstance(MirageSDKNetworkUtils.GetAnkrRPCForSelectedNetwork(networkName), autoSetup);
		}

		private static IMirageSDK CreateMirageSDKInstance(string providerURI, bool autoSetup)
		{
			if (autoSetup)
			{
				MirageSDKAutoCreator.Setup();
			}

			ISilentSigningHandler silentSigningHandler = null;
		#if (UNITY_WEBGL && !UNITY_EDITOR)
			var webGlWrapper = Utils.ConnectProvider<MirageSDK.WebGL.WebGLConnect>.GetConnect().SessionWrapper;
			var contractFunctions = new WebGL.Implementation.ContractFunctionsWebGL(webGlWrapper);
			var eth = new WebGL.Implementation.EthHandlerWebGL(webGlWrapper);
			var walletHandler = (IWalletHandler)webGlWrapper;
			var networkHandler = new WebGL.Implementation.MirageWebGLHelper(webGlWrapper);
		#else
			silentSigningHandler = new MirageSDK.SilentSigning.Implementation.SilentSigningProtocol();
			var web3Provider = new Mobile.MobileWeb3Provider().CreateWeb3(providerURI);
			var contractFunctions = new Mobile.ContractFunctions(web3Provider);
			var eth = new Mobile.EthHandler(web3Provider, silentSigningHandler);
			var walletHandler = new Mobile.MobileWalletHandler();
			var networkHandler = new Mobile.MirageNetworkHelper();
		#endif

			return new MirageSDKWrapper(
				contractFunctions,
				eth,
				walletHandler,
				networkHandler,
				silentSigningHandler
			);
		}
	}
}