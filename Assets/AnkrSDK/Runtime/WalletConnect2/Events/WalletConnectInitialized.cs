using AnkrSDK.Plugins.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.Plugins.WalletConnectSharp.Core;
using AnkrSDK.Runtime.WalletConnect2;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Events
{
    public class WalletConnectInitialized : WalletConnect2TransitionBase
    {
        public WalletConnectInitialized(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnect2Status previousStatus, WalletConnect2Status newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {
            
        }
    }
}