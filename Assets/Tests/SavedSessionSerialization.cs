using System;
using MirageSDK.WalletConnectSharp.Core.Models;
using MirageSDK.WalletConnectSharp.Unity.Utils;
using NUnit.Framework;

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

			var testSession = GetTestSession();

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
			var session1 = GetTestSession();

			Assert.That(session1.Equals(session1));
		}

		[Test]
		public void SavedSession_IsEqualsToNull()
		{
			var session1 = GetTestSession();
			
			Assert.AreNotEqual(session1, null);
		}

		[Test]
		public void SavedSession_IsSessionNotSameToNull()
		{
			var session1 = GetTestSession();
			
			Assert.AreNotSame(session1, null);
		}
		
		[Test]
		public void SavedSession_IsSessionsSame()
		{
			var session1 = GetTestSession();
			
			Assert.AreSame(session1, session1);
		}
		
		[Test]
		public void SavedSession_IsSessionsSameByValue()
		{
			var session1 = GetTestSession();
			var session2 = GetTestSession();
			
			Assert.That(session1 == session2);
		}

		[Test]
		public void SavedSession_TestValueEquality()
		{
			var session1 = GetTestSession();
			var session2 = GetTestSession();

			Assert.AreEqual(session1, session2);
		}

		private static SavedSession GetTestSession()
		{
			var clientMeta = new ClientMeta
			{
				_description = string.Empty,
				_icons = new[] { string.Empty },
				_name = string.Empty,
				_url = string.Empty
			};
			var testSession = new SavedSession(
				string.Empty,
				long.MinValue,
				string.Empty,
				string.Empty,
				new[] { byte.MinValue },
				string.Empty,
				0,
				new[] { string.Empty },
				0,
				clientMeta,
				clientMeta
			);
			return testSession;
		}
	}
}