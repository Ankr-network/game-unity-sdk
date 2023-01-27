using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Unity.Infrastructure
{
	public interface IQuittableComponent
	{
		UniTask Quit();
	}
}