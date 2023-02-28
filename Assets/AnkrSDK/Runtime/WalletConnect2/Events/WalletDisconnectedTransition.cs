using AnkrSDK.WalletConnect.VersionShared.Infrastructure;

namespace AnkrSDK.WalletConnect2.Events
{
    public class WalletDisconnectedTransition : WalletConnect2TransitionBase
    {
        public WalletDisconnectedTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnect2Status previousStatus, WalletConnect2Status newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {
            
        }
    }
}