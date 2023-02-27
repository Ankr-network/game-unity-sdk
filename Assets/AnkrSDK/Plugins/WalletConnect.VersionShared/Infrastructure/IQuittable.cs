using System;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Plugins.WalletConnect.VersionShared.Infrastructure
{
	public interface IQuittable : IDisposable
	{
		UniTask Quit();
	}
}