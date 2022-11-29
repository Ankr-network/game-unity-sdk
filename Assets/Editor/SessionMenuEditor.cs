#if UNITY_EDITOR
using MirageSDK.WalletConnectSharp.Unity.Utils;
using UnityEditor;

public static class SessionMenuEditor
{
	[MenuItem("MirageSDK/Clear Session")]
	public static void ClearSession()
	{
		SessionSaveHandler.ClearSession();
	}
}

#endif