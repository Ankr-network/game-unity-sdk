using System.Linq;
using MirageSDK.Data;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.Core.Utils
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

			object[] firstTopicValue = values[0];
			object secondTopicValue = values.Length > 1 ? values[1] : null;
			object thirdTopicValue = values.Length > 2 ? values[2] : null;

			ethFilterInput.Topics = eventABI.GetTopicBuilder()
				.GetTopics(firstTopicValue, secondTopicValue, thirdTopicValue);

			return ethFilterInput;
		}
	}
}