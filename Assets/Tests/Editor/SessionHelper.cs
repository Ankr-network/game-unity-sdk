using AnkrSDK.WalletConnectSharp.Core.Models;

namespace Tests.Editor
{
	public static class SessionHelper
	{
		public static SavedSession GetTestSession()
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