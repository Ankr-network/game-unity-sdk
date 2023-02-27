using AnkrSDK.Plugins.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.Plugins.WalletConnectSharp.Core;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Events
{
	public class SessionRequestSentTransition : WalletConnectTransitionBase
	{
		public SessionRequestSentTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus) 
			: base(transitionDataProvider, previousStatus, newStatus)
		{
			
		}
	}
}