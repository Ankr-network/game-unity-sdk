using System;
using Cysharp.Threading.Tasks;

namespace MirageSDK.WalletConnect.VersionShared.Infrastructure
{
	public interface IQuittable : IDisposable
	{
		UniTask Quit();
	}
}