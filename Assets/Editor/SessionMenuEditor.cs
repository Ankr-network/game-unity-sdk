#if UNITY_EDITOR
using AnkrSDK.WalletConnectSharp.Unity.Utils;
using UnityEditor;

namespace Editor
{
	public static class SessionMenuEditor
	{
		[MenuItem("AnkrSDK/Clear Session")]
		public static void ClearSession()
		{
			SessionSaveHandler.ClearSession();
		}
	}
}
#endif