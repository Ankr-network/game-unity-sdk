using UnityEngine;
using WalletConnectSharp.Common.Logging;

namespace AnkrSDK.WalletConnect2
{
	public class WalletConnect2Logger : IWC2Logger
	{
		public void Log(string text)
		{
			Debug.Log(text);
		}

		public void LogWarning(string text)
		{
			Debug.LogWarning(text);
		}

		public void LogError(string text)
		{
			Debug.LogError(text);
		}
	}
}