using System;
using AnkrSDK.Core.Infrastructure;
using UnityEngine;

namespace AnkrSDK.SilentSigning.Implementation
{
	public class SilentSigningSessionHandler : ISilentSigningSessionHandler
	{
		private const string SessionKey = "__WALLETCONNECT_SilentSigning_SESSION__";

		public event Action SessionUpdated;

		public string GetSavedSessionSecret()
		{
			return IsSessionSaved() ? PlayerPrefs.GetString(SessionKey) : null;
		}

		public bool IsSessionSaved()
		{
			return PlayerPrefs.HasKey(SessionKey);
		}

		public void SaveSilentSession(string sessionToSave)
		{
			PlayerPrefs.SetString(SessionKey, sessionToSave);
			SessionUpdated?.Invoke();
		}

		public void ClearSilentSession()
		{
			PlayerPrefs.DeleteKey(SessionKey);
			SessionUpdated?.Invoke();
		}
	}
}