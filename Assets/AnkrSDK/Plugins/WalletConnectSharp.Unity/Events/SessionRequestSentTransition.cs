using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.WalletConnectSharp.Core;

namespace AnkrSDK.WalletConnectSharp.Unity.Events
{
	public class SessionRequestSentTransition : WalletConnectTransitionBase
	{
		public SessionRequestSentTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus) 
			: base(transitionDataProvider, previousStatus, newStatus)
		{
			
		}
	}
}