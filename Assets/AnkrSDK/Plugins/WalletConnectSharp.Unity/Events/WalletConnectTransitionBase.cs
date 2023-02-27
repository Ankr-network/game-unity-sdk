using AnkrSDK.Plugins.WalletConnectSharp.Core;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Events
{
    public class WalletConnectTransitionBase
    {
        public WalletConnectStatus PreviousStatus { get; }
        public WalletConnectStatus NewStatus { get; }

        public WalletConnectTransitionBase(WalletConnectSession session, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
        {
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
        }
    }
}