using System;
using AnkrSDK.WalletConnectSharp.Core;

namespace AnkrSDK.WalletConnectSharp.Unity.Events
{
    public static class TransitionDataFactory
    {
        public static IWalletConnectTransitionData CreateTransitionData(WalletConnectStatus previousStatus,
            WalletConnectStatus newStatus, WalletConnectSession session)
        {
            if (previousStatus == WalletConnectStatus.Uninitialized)
            {
                return new SessionCreatedTransitionData(session);
            }
            
            switch (newStatus)
            {
                case WalletConnectStatus.DisconnectedSessionCached:
                case WalletConnectStatus.DisconnectedNoSession:
                {   
                    return new WalletDisconnectedTransitionData(session);
                }
                case WalletConnectStatus.TransportConnected:
                {
                    return new TransportConnectedTransitionData(session);
                }
                case WalletConnectStatus.SessionConnected:
                {
                    return new SessionConnectedTransitionData(session);
                }
                case WalletConnectStatus.WalletConnected:
                {
                    return new WalletConnectedTransitionData(session);
                }
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}