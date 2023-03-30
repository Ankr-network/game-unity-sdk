using MirageSDK.WalletConnect.VersionShared.Infrastructure;
using MirageSDK.WalletConnectSharp.Core;

namespace MirageSDK.WalletConnectSharp.Unity.Events
{
    public class WalletDisconnectedTransition : WalletConnectTransitionBase
    {
        public WalletDisconnectedTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {
            
        }
    }
}