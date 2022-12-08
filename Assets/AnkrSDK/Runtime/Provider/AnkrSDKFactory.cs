using System;
using AnkrSDK.Core;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity;

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
			return CreateAnkrSDKInstance(AnkrSDKFactoryHelper.GetAnkrRPCForSelectedNetwork(networkName), autoSetup);
		}

		private static IAnkrSDK CreateAnkrSDKInstance(string providerURI, bool autoSetup)
		{
			if (autoSetup)
			{
				AnkrSDKAutoCreator.Setup();
			}

			var silentSigningHandler = new SilentSigning.SilentSigningProtocol();

		#if (UNITY_WEBGL && !UNITY_EDITOR)
			var webGlWrapper = Utils.ConnectProvider<WebGL.WebGLConnect>.GetWalletConnect().SessionWrapper;
			var contractFunctions = new WebGL.Implementation.ContractFunctionsWebGL(webGlWrapper);
			var eth = new WebGL.Implementation.EthHandlerWebGL(webGlWrapper);
			var walletHandler = (IWalletHandler)webGlWrapper;
			var networkHandler = new WebGL.Implementation.AnkrNetworkWebGLHelper(webGlWrapper);
		#else
			var web3Provider = new Mobile.MobileWeb3Provider().CreateWeb3(providerURI);
			var contractFunctions = new Mobile.ContractFunctions(web3Provider);
			var eth = new Mobile.EthHandler(web3Provider);
			var walletHandler = new Mobile.MobileWalletHandler();
			var networkHandler = new Mobile.AnkrNetworkHelper();
		#endif
			SetupDeeplinkOnEachMessage(silentSigningHandler.IsSilentSigningActive);

			return new AnkrSDKWrapper(
				contractFunctions,
				eth,
				walletHandler,
				networkHandler,
				silentSigningHandler
			);
		}

		private static void SetupDeeplinkOnEachMessage(Func<bool> isSilentSigningActive)
		{
			var walletConnect = ConnectProvider<WalletConnect>.GetWalletConnect();
			walletConnect.SessionUpdated += () => OnSessionUpdated(isSilentSigningActive);
			if (walletConnect.Session != null)
			{
				SubscribeSession(walletConnect.Session, isSilentSigningActive);
			}
		}

		private static void OnSessionUpdated(Func<bool> isSilentSigningActive)
		{
			var walletConnect = ConnectProvider<WalletConnect>.GetWalletConnect();
			SubscribeSession(walletConnect.Session, isSilentSigningActive);
		}

		private static void SubscribeSession(WalletConnectSession session, Func<bool> isSilentSigningActive)
		{
			if (!isSilentSigningActive())
			{
				session.OnSend += OnSessionSend;
			}
		}

		private static void OnSessionSend(object sender, WalletConnectSession e)
		{
			var walletConnect = ConnectProvider<WalletConnect>.GetWalletConnect();
			walletConnect.OpenMobileWallet();
		}
	}
}