using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Data
{
	public class EventFilterRequest<TEvDTO>
	{
		public TEvDTO request { get; set; }
		public BlockParameter fromBlock { get; set; }
		public BlockParameter toBlock { get; set; }
	}
}