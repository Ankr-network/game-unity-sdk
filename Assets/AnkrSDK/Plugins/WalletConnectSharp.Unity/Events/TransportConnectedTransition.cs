using AnkrSDK.Plugins.WalletConnectSharp.Core;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Events
{
    public class TransportConnectedTransition : WalletConnectTransitionBase
    {
        public TransportConnectedTransition(WalletConnectSession session, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(session, previousStatus, newStatus)
        {
            
        }
    }
}