using System.Linq;
using AnkrSDK.Data;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Utils
{
	public static class EventFilterHelper
	{
		public static NewFilterInput CreateEventFilters<TEvDto>(string contractAddress, EventFilterData evFilter = null)
		{
			var ethFilterInput =
				FilterInputBuilder.GetDefaultFilterInput(contractAddress, evFilter?.FromBlock, evFilter?.ToBlock);
			if (evFilter == null || !evFilter.AreTopicsFilled())
			{
				return ethFilterInput;
			}

			var eventABI = ABITypedRegistry.GetEvent<TEvDto>();

			ethFilterInput.Topics = eventABI.GetTopicBuilder()
				.GetTopics(evFilter.FilterTopic1, evFilter.FilterTopic2, evFilter.FilterTopic3);

			return ethFilterInput;
		}

		public static NewFilterInput CreateEventFilters<TEvDto>(string contractAddress,
			EventFilterRequest<TEvDto> evFilter = null)
		{
			var values = evFilter?.AssembleTopics();

			var ethFilterInput =
				FilterInputBuilder.GetDefaultFilterInput(contractAddress, evFilter?.FromBlock, evFilter?.ToBlock);
			if (evFilter == null || !values.Any())
			{
				return ethFilterInput;
			}

			var eventABI = ABITypedRegistry.GetEvent<TEvDto>();

			ethFilterInput.Topics = eventABI.GetTopicBuilder()
				.GetTopics(values[0], values[1], values[2]);

			return ethFilterInput;
		}
	}
}