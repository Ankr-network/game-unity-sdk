using System;
using MirageSDK.WalletConnectSharp.Core.Models;
using UnityEngine.Events;

namespace MirageSDK.WalletConnectSharp.Unity
{
	[Serializable]
	public class WalletConnectEventWithSessionData : UnityEvent<WCSessionData>
	{
	}
}