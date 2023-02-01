using System;
using AnkrSDK.WalletConnectSharp.Core;
using UnityEngine.Events;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	[Serializable]
	public class WalletConnectEventWithSession : UnityEvent<WalletConnectSession>
	{
	}
}