using AnkrSDK.WalletConnect.VersionShared.Models;

namespace AnkrSDK.WalletConnect.VersionShared.Infrastructure
{
	public interface IErrorHolder
	{
		JsonRpcResponse.JsonRpcError Error { get; }
		bool IsError { get; }
	}
}