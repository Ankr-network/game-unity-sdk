using AnkrSDK.Plugins.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.Plugins.WalletConnectSharp.Core;
using AnkrSDK.Runtime.WalletConnect2;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Events
{
	public class SessionRequestSentTransition : WalletConnect2TransitionBase
	{
		public SessionRequestSentTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnect2Status previousStatus, WalletConnect2Status newStatus) 
			: base(transitionDataProvider, previousStatus, newStatus)
		{
			
		}
	}
}