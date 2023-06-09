using MirageSDK.WalletConnect.VersionShared.Infrastructure;

namespace MirageSDK.WalletConnectSharp.Core.StatusEvents
{
    public class SessionCreatedTransition : WalletConnectTransitionBase
    {
        public SessionCreatedTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {

        }
    }
}