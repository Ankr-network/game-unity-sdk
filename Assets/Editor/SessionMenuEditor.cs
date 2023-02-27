#if UNITY_EDITOR
using AnkrSDK.Plugins.WalletConnectSharp.Unity.Utils;
using UnityEditor;

public static class SessionMenuEditor
{
	[MenuItem("AnkrSDK/Clear Session")]
	public static void ClearSession()
	{
		SessionSaveHandler.ClearSession();
	}
}

#endif