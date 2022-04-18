using System.Collections.Generic;
using System.Linq;
using AnkrSDK.Core.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;
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
			var values = GetDataFromRequest(evFilter);
			
			var eventABI = ABITypedRegistry.GetEvent<TEvDto>();
			
			var ethFilterInput = FilterInputBuilder.GetDefaultFilterInput(contractAddress, evFilter?.fromBlock, evFilter?.toBlock);
			if (evFilter != null && values.Any())
			{
				ethFilterInput.Topics = eventABI.GetTopicBuilder()
					.GetTopics(
						ReturnArrayOrNull(values[0]),
						ReturnArrayOrNull(values[1]),
						ReturnArrayOrNull(values[2])
						);
			}

			return ethFilterInput;
		}

		private static object[] ReturnArrayOrNull(object value)
		{
			return value != null ? new[] {value} : null;
		}

		private static List<object> GetDataFromRequest<TEvDto>(EventFilterRequest<TEvDto> evFilter)
		{
			var properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(typeof(TEvDto));
			
			return properties.Select(property => property.GetValue(evFilter, null)).ToList();
		}	
	}
}