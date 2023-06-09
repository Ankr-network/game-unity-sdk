using MirageSDK.WalletConnect.VersionShared.Infrastructure;

namespace MirageSDK.WalletConnectSharp.Core.StatusEvents
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