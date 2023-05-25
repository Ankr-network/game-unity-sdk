using MirageSDK.WalletConnect.VersionShared.Infrastructure;

namespace MirageSDK.WalletConnectSharp.Core.StatusEvents
{
    public class WalletConnectedTransition : WalletConnectTransitionBase
    {
        public WalletConnectedTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {

        }
    }
}