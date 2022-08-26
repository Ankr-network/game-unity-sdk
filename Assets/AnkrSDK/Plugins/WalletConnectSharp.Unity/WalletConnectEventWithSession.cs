using System;
using UnityEngine.Events;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	[Serializable]
	public class WalletConnectEventWithSession : UnityEvent<WalletConnectUnitySession>
	{
	}
}