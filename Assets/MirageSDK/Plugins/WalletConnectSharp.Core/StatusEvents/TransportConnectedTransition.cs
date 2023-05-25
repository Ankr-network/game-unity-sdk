using MirageSDK.WalletConnect.VersionShared.Infrastructure;

namespace MirageSDK.WalletConnectSharp.Core.StatusEvents
{
    public class TransportConnectedTransition : WalletConnectTransitionBase
    {
        public TransportConnectedTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {

        }
    }
}