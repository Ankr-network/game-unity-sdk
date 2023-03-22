using MirageSDK.WalletConnect.VersionShared.Infrastructure;
using MirageSDK.WalletConnectSharp.Core;

namespace MirageSDK.WalletConnectSharp.Unity.Events
{
    public class SessionCreatedTransition : WalletConnectTransitionBase
    {
        public SessionCreatedTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {
            
        }
    }
}