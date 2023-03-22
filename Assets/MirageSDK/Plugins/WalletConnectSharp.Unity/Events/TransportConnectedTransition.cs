using MirageSDK.WalletConnect.VersionShared.Infrastructure;
using MirageSDK.WalletConnectSharp.Core;

namespace MirageSDK.WalletConnectSharp.Unity.Events
{
    public class TransportConnectedTransition : WalletConnectTransitionBase
    {
        public TransportConnectedTransition(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnectStatus previousStatus, WalletConnectStatus newStatus)
            : base(transitionDataProvider, previousStatus, newStatus)
        {
            
        }
    }
}