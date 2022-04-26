using System.Linq;
using AnkrSDK.Core.Data;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Utils
{
	public static class EventFilterHelper
	{
		public static NewFilterInput CreateEventFilters<TEvDto>(string contractAddress, EventFilterData evFilter = null)
		{
			var eventABI = ABITypedRegistry.GetEvent<TEvDto>();
			
			var ethFilterInput = FilterInputBuilder.GetDefaultFilterInput(contractAddress, evFilter?.fromBlock, evFilter?.toBlock);
			if (evFilter != null && evFilter.AreTopicsFilled())
			{
				ethFilterInput.Topics = eventABI.GetTopicBuilder()
					.GetTopics(evFilter.filterTopic1, evFilter.filterTopic2, evFilter.filterTopic3);
			}

			return ethFilterInput;
		}
		
		public static NewFilterInput CreateEventFilters<TEvDto>(string contractAddress, EventFilterRequest<TEvDto> evFilter = null)
		{
			var values = evFilter.AssembleTopics();
			
			var eventABI = ABITypedRegistry.GetEvent<TEvDto>();
			
			var ethFilterInput = FilterInputBuilder.GetDefaultFilterInput(contractAddress, evFilter?.FromBlock, evFilter?.ToBlock);
			if (evFilter != null && values.Any())
			{
				ethFilterInput.Topics = eventABI.GetTopicBuilder()
					.GetTopics(values[0], values[1], values[2]);
			}

			return ethFilterInput;
		}
	}
}