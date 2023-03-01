using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.WalletConnectSharp.Core;

namespace AnkrSDK.WalletConnectSharp.Unity.Events
{
    public class WalletConnectTransitionBase
    {
        public WalletConnectStatus PreviousStatus { get; }
        public WalletConnectStatus NewStatus { get; }

        public WalletConnectTransitionBase(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
        {
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
        }
    }
}