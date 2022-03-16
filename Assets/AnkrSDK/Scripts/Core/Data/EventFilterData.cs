using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Data
{
	public class EventFilterData
	{
		public object[] filterTopic1;
		public object[] filterTopic2;
		public object[] filterTopic3;
		public BlockParameter fromBlock;
		public BlockParameter toBlock;
	}
}