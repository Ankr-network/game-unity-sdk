using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.WalletConnectSharp.Core;

namespace AnkrSDK.WalletConnectSharp.Unity.Events
{
    public class WalletConnectedTransition : WalletConnectTransitionBase
    {
        public WalletConnectedTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {
            
        }
    }
}