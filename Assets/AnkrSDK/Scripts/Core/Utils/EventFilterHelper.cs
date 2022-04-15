using AnkrSDK.Core.Data;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Utils
{
	public static class EventFilterHelper
	{
		public static NewFilterInput CreateEventFilters<TEvDto>(string contractAddress, EventFilterData evFilter = null)
		{
			var eventABI = ABITypedRegistry.GetEvent(typeof(TEvDto));
			
			var ethFilterInput = FilterInputBuilder.GetDefaultFilterInput(contractAddress, evFilter.fromBlock, evFilter.toBlock);
			if (evFilter.filterTopic1 != null || evFilter.filterTopic2 != null || evFilter.filterTopic3 != null)
			{
				ethFilterInput.Topics = eventABI.GetTopicBuilder()
					.GetTopics(evFilter.filterTopic1, evFilter.filterTopic2, evFilter.filterTopic3);
			}

			return ethFilterInput;
		}
	}
}