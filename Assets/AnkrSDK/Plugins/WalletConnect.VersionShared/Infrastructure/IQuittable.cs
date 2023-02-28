using System;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnect.VersionShared.Infrastructure
{
	public interface IQuittable : IDisposable
	{
		UniTask Quit();
	}
}