using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Data;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Utils
{
	public static class EventFilterHelper
	{
		public static NewFilterInput CreateEventFilters(EventBase eventHandler, EventFilterData evFilter = null)
		{
			if (evFilter == null)
			{
				return eventHandler.CreateFilterInput();
			}

			var isFilterTopic1Filled = evFilter.filterTopic1 != null;
			var isFilterTopic2Filled = evFilter.filterTopic2 != null;
			var isFilterTopic3Filled = evFilter.filterTopic3 != null;
			
			if (isFilterTopic3Filled && isFilterTopic2Filled && isFilterTopic1Filled)
			{
				return eventHandler.CreateFilterInput(evFilter.filterTopic1, evFilter.filterTopic2,
					evFilter.filterTopic3, evFilter.fromBlock, evFilter.toBlock);
			}

			if (isFilterTopic2Filled && isFilterTopic1Filled)
			{
				return eventHandler.CreateFilterInput(evFilter.filterTopic1, evFilter.filterTopic2,
					evFilter.fromBlock, evFilter.toBlock);
			}

			if (isFilterTopic1Filled)
			{
				return eventHandler.CreateFilterInput(evFilter.filterTopic1, evFilter.fromBlock,
					evFilter.toBlock);
			}

			return eventHandler.CreateFilterInput(evFilter.fromBlock, evFilter.toBlock);
		}
	}
}