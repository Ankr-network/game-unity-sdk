using System;
using MirageSDK.WalletConnect.VersionShared.Infrastructure;

namespace MirageSDK.WalletConnectSharp.Core.StatusEvents
{
    public static class TransitionDataFactory
    {
        public static WalletConnectTransitionBase CreateTransitionObj(WalletConnectStatus previousStatus,
            WalletConnectStatus newStatus, IWalletConnectTransitionDataProvider session)
        {
            if (previousStatus == WalletConnectStatus.Uninitialized)
            {
                return new SessionCreatedTransition(session, previousStatus, newStatus);
            }

            switch (newStatus)
            {
                case WalletConnectStatus.DisconnectedSessionCached:
                case WalletConnectStatus.DisconnectedNoSession:
                {
                    return new WalletDisconnectedTransition(session, previousStatus, newStatus);
                }
                case WalletConnectStatus.TransportConnected:
                {
                    return new TransportConnectedTransition(session, previousStatus, newStatus);
                }
                case WalletConnectStatus.SessionRequestSent:
                {
                    return new SessionRequestSentTransition(session, previousStatus, newStatus);
                }
                case WalletConnectStatus.WalletConnected:
                {
                    return new WalletConnectedTransition(session, previousStatus, newStatus);
                }
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}