using System.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Unity.Infrastructure
{
	public interface IQuittableComponent
	{
		Task Quit();
	}
}