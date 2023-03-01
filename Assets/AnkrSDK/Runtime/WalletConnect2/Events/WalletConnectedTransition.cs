using AnkrSDK.WalletConnect.VersionShared.Infrastructure;

namespace AnkrSDK.WalletConnect2.Events
{
    public class WalletConnectedTransition : WalletConnect2TransitionBase
    {
        public WalletConnectedTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnect2Status previousStatus, WalletConnect2Status newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {
            
        }
    }
}