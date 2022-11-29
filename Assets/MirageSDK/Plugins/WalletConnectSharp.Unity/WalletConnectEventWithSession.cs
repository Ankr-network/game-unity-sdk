using System;
using UnityEngine.Events;

namespace MirageSDK.WalletConnectSharp.Unity
{
	[Serializable]
	public class WalletConnectEventWithSession : UnityEvent<WalletConnectUnitySession>
	{
	}
}