using AnkrSDK.WalletConnect.VersionShared.Infrastructure;

namespace AnkrSDK.WalletConnect2.Events
{
    public class WalletConnectInitialized : WalletConnect2TransitionBase
    {
        public WalletConnectInitialized(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnect2Status previousStatus, WalletConnect2Status newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {
            
        }
    }
}