using AnkrSDK.Plugins.WalletConnectSharp.Core;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Events
{
    public class SessionCreatedTransition : WalletConnectTransitionBase
    {
        public SessionCreatedTransition(WalletConnectSession session, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(session, previousStatus, newStatus)
        {
            
        }
    }
}