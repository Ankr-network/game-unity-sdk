using AnkrSDK.WalletConnectSharp.Core;

namespace AnkrSDK.WalletConnectSharp.Unity.Events
{
    public class WalletConnectTransitionBase
    {
        public WalletConnectStatus PreviousStatus { get; }
        public WalletConnectStatus NewStatus { get; }

        public WalletConnectTransitionBase(WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
        {
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
        }
    }
}