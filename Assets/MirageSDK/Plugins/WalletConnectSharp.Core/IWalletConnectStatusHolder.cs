using System;
using MirageSDK.WalletConnectSharp.Core.StatusEvents;

namespace MirageSDK.WalletConnectSharp.Core
{
	public interface IWalletConnectStatusHolder
	{
		event Action<WalletConnectTransitionBase> SessionStatusUpdated;
		WalletConnectStatus Status { get; }
	}
}