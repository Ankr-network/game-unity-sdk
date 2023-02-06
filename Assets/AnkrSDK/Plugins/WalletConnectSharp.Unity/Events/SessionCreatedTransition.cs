using AnkrSDK.WalletConnectSharp.Core;

namespace AnkrSDK.WalletConnectSharp.Unity.Events
{
    public class SessionCreatedTransition : WalletConnectTransitionBase
    {
        public SessionCreatedTransition(WalletConnectSession session, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(session, previousStatus, newStatus)
        {
            
        }
    }
}