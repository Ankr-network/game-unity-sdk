using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.Data
{
	public class EventFilterData
	{
		public object[] FilterTopic1 { get; set; }
		public object[] FilterTopic2 { get; set; }
		public object[] FilterTopic3 { get; set; }
		public BlockParameter FromBlock { get; set; }
		public BlockParameter ToBlock { get; set; }

		public bool AreTopicsFilled()
		{
			return FilterTopic1 != null || FilterTopic2 != null || FilterTopic3 != null;
		}
	}
}