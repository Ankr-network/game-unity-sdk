using System;

namespace AnkrSDK.Core.Infrastructure
{
	public interface ISilentSigningSessionHandler
	{
		event Action SessionUpdated;

		bool IsSessionSaved();
		void SaveSilentSession(string sessionToSave);
		void ClearSilentSession();
		string GetSavedSessionSecret();
	}
}