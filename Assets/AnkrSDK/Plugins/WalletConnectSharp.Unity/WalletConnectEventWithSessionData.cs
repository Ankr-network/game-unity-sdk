using System;
using AnkrSDK.WalletConnectSharp.Core.Models;
using UnityEngine.Events;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	[Serializable]
	public class WalletConnectEventWithSessionData : UnityEvent<WCSessionData>
	{
	}
}