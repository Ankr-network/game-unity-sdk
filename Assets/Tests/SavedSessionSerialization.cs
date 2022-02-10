using System;
using NUnit.Framework;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;

namespace Tests
{
	public class SavedSessionSerialization
	{
		[Test]
		public void SavedSession_ClearSession()
		{
			if (!WalletConnect.IsSessionSaved())
			{
				return;
			}

			var savedSession = WalletConnect.GetSavedSession();
			WalletConnect.ClearSession();
			var savedSessionAfterClear = WalletConnect.GetSavedSession();
			Assert.IsNull(savedSessionAfterClear);
			WalletConnect.SaveSession(savedSession);
		}

		[Test]
		public void SavedSession_SaveSession()
		{
			SavedSession savedSessionBeforeTest = null;
			if (WalletConnect.IsSessionSaved())
			{
				savedSessionBeforeTest = WalletConnect.GetSavedSession();
			}

			var testSession = GetTestSession();

			WalletConnect.SaveSession(testSession);
			Assert.That(WalletConnect.IsSessionSaved, "Session was not saved at all");
			var sessionAfterSave = WalletConnect.GetSavedSession();
			Assert.That(sessionAfterSave.Equals(testSession));

			if (savedSessionBeforeTest != null)
			{
				WalletConnect.SaveSession(savedSessionBeforeTest);
			}
			else
			{
				WalletConnect.ClearSession();
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
				Description = string.Empty,
				Icons = new[] { string.Empty },
				Name = string.Empty,
				URL = string.Empty
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