using AnkrSDK.WalletConnectSharp.Core;

namespace AnkrSDK.WalletConnectSharp.Unity.Events
{
    public readonly struct WalletConnectTransition
    {
        public readonly WalletConnectStatus PreviousStatus;
        public readonly WalletConnectStatus NewStatus;
        public readonly IWalletConnectTransitionData TransitionData;

        public WalletConnectTransition(WalletConnectStatus previousStatus, WalletConnectStatus newStatus,
            IWalletConnectTransitionData transitionData, WalletConnectSession session)
        {
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            TransitionData = transitionData;
        }
    }
}