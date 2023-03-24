using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MirageSDK.WalletConnectSharp.Unity.Network.Client
{
	public class WaitForBackgroundThread
	{
		public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
		{
			return Task.Run(() => { }).ConfigureAwait(false).GetAwaiter();
		}
	}
}