using System.Collections.Generic;
using WalletConnectSharp.Sign.Interfaces;

namespace AnkrSDK.Runtime.WalletConnect2.RpcRequests
{
	public class RpcRequestDataBase : Dictionary<string, object>, IWcMethod
	{
		
	}
}