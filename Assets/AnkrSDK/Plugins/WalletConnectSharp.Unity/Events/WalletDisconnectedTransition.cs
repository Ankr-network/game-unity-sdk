using AnkrSDK.WalletConnectSharp.Core;

namespace AnkrSDK.WalletConnectSharp.Unity.Events
{
    public class WalletDisconnectedTransition : WalletConnectTransitionBase
    {
        public WalletDisconnectedTransition(WalletConnectSession session, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(previousStatus, newStatus)
        {
            
        }
    }
}