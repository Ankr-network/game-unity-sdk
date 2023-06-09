using MirageSDK.WalletConnect.VersionShared.Infrastructure;

namespace MirageSDK.WalletConnectSharp.Core.StatusEvents
{
	public class SessionRequestSentTransition : WalletConnectTransitionBase
	{
		public SessionRequestSentTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
			: base(transitionDataProvider, previousStatus, newStatus)
		{

		}
	}
}