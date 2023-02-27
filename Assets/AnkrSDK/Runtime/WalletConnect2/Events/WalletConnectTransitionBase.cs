using AnkrSDK.Plugins.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.Plugins.WalletConnectSharp.Core;
using AnkrSDK.Runtime.WalletConnect2;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity.Events
{
    public class WalletConnect2TransitionBase
    {
        public WalletConnect2Status PreviousStatus { get; }
        public WalletConnect2Status NewStatus { get; }

        public WalletConnect2TransitionBase(IWalletConnectTransitionDataProvider transitionDataProvider, WalletConnect2Status previousStatus, WalletConnect2Status newStatus)
        {
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
        }
    }
}