using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client.Helpers
{
	public class WaitForBackgroundThread
	{
		public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
		{
			return Task.Run(() => { }).ConfigureAwait(false).GetAwaiter();
		}
	}
}