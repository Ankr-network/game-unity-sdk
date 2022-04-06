using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Unity.Utils;
using NUnit.Framework;
using TestUtils;

namespace Tests
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

			var testSession = TestHelper.GetTestSession();

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
			var session = TestHelper.GetTestSession();

			Assert.That(session.Equals(session));
		}

		[Test]
		public void SavedSession_IsEqualsToNull()
		{
			var session = TestHelper.GetTestSession();
			
			Assert.AreNotEqual(session, null);
		}

		[Test]
		public void SavedSession_IsSessionNotSameToNull()
		{
			var session = TestHelper.GetTestSession();
			
			Assert.AreNotSame(session, null);
		}
		
		[Test]
		public void SavedSession_IsSessionsSame()
		{
			var session = TestHelper.GetTestSession();
			
			Assert.AreSame(session, session);
		}
		
		[Test]
		public void SavedSession_IsSessionsSameByValue()
		{
			var session1 = TestHelper.GetTestSession();
			var session2 = TestHelper.GetTestSession();
			
			Assert.That(session1 == session2);
		}

		[Test]
		public void SavedSession_TestValueEquality()
		{
			var session1 = TestHelper.GetTestSession();
			var session2 = TestHelper.GetTestSession();

			Assert.AreEqual(session1, session2);
		}
	}
}