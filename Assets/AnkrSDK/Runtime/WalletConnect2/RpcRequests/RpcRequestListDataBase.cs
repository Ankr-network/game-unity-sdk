using System.Collections.Generic;
using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using Newtonsoft.Json;
using WalletConnectSharp.Sign.Interfaces;

namespace AnkrSDK.WalletConnect2.RpcRequests
{
	public class RpcRequestListDataBase : List<object>, IWcMethod, IIdentifiable
	{
		//this id is required for WC and WC2 to have a unified interface, 
		//for WC2 it is set within WalletConnect.Sign during WC2 request via reflection

		[JsonIgnore]
		private long _id;
		
		[JsonIgnore] 
		public long ID => _id;
	}
}