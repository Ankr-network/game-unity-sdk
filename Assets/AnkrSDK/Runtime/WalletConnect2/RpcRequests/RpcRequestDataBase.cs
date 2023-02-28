using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using Newtonsoft.Json;
using WalletConnectSharp.Sign.Interfaces;

namespace AnkrSDK.WalletConnect2.RpcRequests
{
	public class RpcRequestDataBase : IWcMethod, Identifiable
	{
		//this id is only required for WC and WC2 to have a unified interface, 
		//for WC2 request ids are assigned by internal WalletConnectSharp request engine
		[JsonIgnore] public long ID => 0;
	}
}