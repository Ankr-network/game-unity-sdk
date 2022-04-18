using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Data
{
	public class EventFilterData
	{
		public object[] filterTopic1 { get; set; }
		public object[] filterTopic2 { get; set; }
		public object[] filterTopic3 { get; set; }
		public BlockParameter fromBlock { get; set; }
		public BlockParameter toBlock { get; set; }

		public bool TopicsIsFilled()
		{
			return filterTopic1 != null || filterTopic2 != null || filterTopic3 != null;
		}
	}
	
	public class EventFilterRequest<TEvDTO>
	{
		public TEvDTO request { get; set; }
		public BlockParameter fromBlock { get; set; }
		public BlockParameter toBlock { get; set; }
	}
}