using AnkrSDK.Plugins.WalletConnectSharp.Core;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Events
{
	public class SessionRequestSentTransition : WalletConnectTransitionBase
	{
		public SessionRequestSentTransition(WalletConnectSession session, WalletConnectStatus previousStatus, WalletConnectStatus newStatus) 
			: base(session, previousStatus, newStatus)
		{
			
		}
	}
}