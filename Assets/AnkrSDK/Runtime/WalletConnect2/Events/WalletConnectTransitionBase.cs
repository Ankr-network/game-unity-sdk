using AnkrSDK.WalletConnect.VersionShared.Infrastructure;

namespace AnkrSDK.WalletConnect2.Events
{
    public class WalletConnect2TransitionBase
    {
        public WalletConnect2Status PreviousStatus { get; }
        public WalletConnect2Status NewStatus { get; }

        public WalletConnect2TransitionBase(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnect2Status previousStatus, WalletConnect2Status newStatus)
        {
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
        }
    }
}