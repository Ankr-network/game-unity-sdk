using AnkrSDK.WalletConnect.VersionShared.Infrastructure;

namespace AnkrSDK.WalletConnect2.Events
{
	public class SessionRequestSentTransition : WalletConnect2TransitionBase
	{
		public SessionRequestSentTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnect2Status previousStatus, WalletConnect2Status newStatus) 
			: base(transitionDataProvider, previousStatus, newStatus)
		{
			
		}
	}
}