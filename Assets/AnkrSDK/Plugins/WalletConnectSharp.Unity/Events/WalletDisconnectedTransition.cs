using AnkrSDK.Plugins.WalletConnectSharp.Core;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Events
{
    public class WalletDisconnectedTransition : WalletConnectTransitionBase
    {
        public WalletDisconnectedTransition(WalletConnectSession session, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(session, previousStatus, newStatus)
        {
            
        }
    }
}