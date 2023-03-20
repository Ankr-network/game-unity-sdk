using MirageSDK.WalletConnect.VersionShared.Models;

namespace MirageSDK.WalletConnect.VersionShared.Infrastructure
{
	public interface IErrorHolder
	{
		JsonRpcResponse.JsonRpcError Error { get; }
		bool IsError { get; }
	}
}