#if UNITY_EDITOR
using MirageSDK.WalletConnectSharp.Unity.Utils;
using UnityEditor;

namespace Editor
{
	public static class SessionMenuEditor
	{
		[MenuItem("MirageSDK/Clear Session")]
		public static void ClearSession()
		{
			SessionSaveHandler.ClearSession();
		}
	}
}

#endif