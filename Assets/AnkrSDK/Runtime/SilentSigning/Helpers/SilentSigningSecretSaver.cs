using System;
using UnityEngine;

namespace AnkrSDK.SilentSigning.Helpers
{
	public class SilentSigningSecretSaver
	{
		private const string SessionKey = "__WALLETCONNECT_SilentSigning_SESSION__";

		public static event Action SessionUpdated;

		public static string GetSavedSessionSecret()
		{
			return IsSessionSaved() ? PlayerPrefs.GetString(SessionKey) : null;
		}

		public static bool IsSessionSaved()
		{
			return PlayerPrefs.HasKey(SessionKey);
		}

		public static void SaveSilentSession(string sessionToSave)
		{
			PlayerPrefs.SetString(SessionKey, sessionToSave);
			SessionUpdated?.Invoke();
		}

		public static void ClearSilentSession()
		{
			PlayerPrefs.DeleteKey(SessionKey);
			SessionUpdated?.Invoke();
		}
	}
}