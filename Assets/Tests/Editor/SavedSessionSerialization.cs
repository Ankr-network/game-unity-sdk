using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Unity.Utils;
using NUnit.Framework;

namespace Tests.Editor
{
	public class SavedSessionSerialization
	{
		[Test]
		public void SavedSession_ClearSession()
		{
			if (!SessionSaveHandler.IsSessionSaved())
			{
				return;
			}

			var savedSession = SessionSaveHandler.GetSavedSession();
			SessionSaveHandler.ClearSession();
			var savedSessionAfterClear = SessionSaveHandler.GetSavedSession();
			Assert.IsNull(savedSessionAfterClear);
			SessionSaveHandler.SaveSession(savedSession);
		}

		[Test]
		public void SavedSession_SaveSession()
		{
			SavedSession savedSessionBeforeTest = null;
			if (SessionSaveHandler.IsSessionSaved())
			{
				savedSessionBeforeTest = SessionSaveHandler.GetSavedSession();
			}

			var testSession = SessionHelper.GetTestSession();

			SessionSaveHandler.SaveSession(testSession);
			Assert.That(SessionSaveHandler.IsSessionSaved, "Session was not saved at all");
			var sessionAfterSave = SessionSaveHandler.GetSavedSession();
			Assert.That(sessionAfterSave.Equals(testSession));

			if (savedSessionBeforeTest != null)
			{
				SessionSaveHandler.SaveSession(savedSessionBeforeTest);
			}
			else
			{
				SessionSaveHandler.ClearSession();
			}
		}

		[Test]
		public void SavedSession_TestEquality()
		{
			var session = SessionHelper.GetTestSession();

			Assert.That(session.Equals(session));
		}

		[Test]
		public void SavedSession_IsEqualsToNull()
		{
			var session = SessionHelper.GetTestSession();
			
			Assert.AreNotEqual(session, null);
		}

		[Test]
		public void SavedSession_IsSessionNotSameToNull()
		{
			var session = SessionHelper.GetTestSession();
			
			Assert.AreNotSame(session, null);
		}
		
		[Test]
		public void SavedSession_IsSessionsSame()
		{
			var session = SessionHelper.GetTestSession();
			
			Assert.AreSame(session, session);
		}
		
		[Test]
		public void SavedSession_IsSessionsSameByValue()
		{
			var session1 = SessionHelper.GetTestSession();
			var session2 = SessionHelper.GetTestSession();
			
			Assert.That(session1 == session2);
		}

		[Test]
		public void SavedSession_TestValueEquality()
		{
			var session1 = SessionHelper.GetTestSession();
			var session2 = SessionHelper.GetTestSession();

			Assert.AreEqual(session1, session2);
		}
	}
}