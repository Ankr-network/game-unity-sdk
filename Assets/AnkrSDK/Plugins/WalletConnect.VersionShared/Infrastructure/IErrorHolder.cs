using AnkrSDK.Plugins.WalletConnect.VersionShared.Models;

namespace AnkrSDK.Plugins.WalletConnect.VersionShared.Infrastructure
{
	public interface IErrorHolder
	{
		JsonRpcResponse.JsonRpcError Error { get; }
		bool IsError { get; }
	}
}