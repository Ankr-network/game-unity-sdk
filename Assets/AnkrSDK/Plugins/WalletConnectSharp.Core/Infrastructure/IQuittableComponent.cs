using System;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Core.Infrastructure
{
	public interface IQuittableComponent : IDisposable
	{
		UniTask Quit();
	}
}