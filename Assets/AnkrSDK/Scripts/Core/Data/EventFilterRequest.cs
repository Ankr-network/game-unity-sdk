using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Data
{	
	public class EventFilterRequest<TEvDto>
	{
		private class Topic
		{
			public string Name { get; set; }
			public string Alias { get; set; }
			public object Value { get; set; }
		}
		
		public BlockParameter FromBlock { get; set; }
		public BlockParameter ToBlock { get; set; }
		private readonly List<Topic> _topics;
		
		public EventFilterRequest()
		{
			_topics = CollectTopics();
		}

		public void SetFromBlock(BlockParameter fromBlock)
		{
			FromBlock = fromBlock;
		}
		
		public void SetToBlock(BlockParameter toBlock)
		{
			ToBlock = toBlock;
		}

		public void AddTopic(string name, object value)
		{
			var topic = FindTopicByName(name);
			if (topic != null)
			{
				topic.Value = value;
			}
			else
			{
				throw new Exception("Topic name is not allowed");
			}
		}

		public object[][] AssembleTopics()
		{
			return _topics.Select(topic =>
			{
				return topic.Value != null ? new[] { topic.Value } : null;
			}).ToArray();
		}

		private Topic FindTopicByName(string name)
		{
			return _topics.FirstOrDefault(topic => topic.Name == name || topic.Alias == name);
		} 
		
		private List<Topic> CollectTopics()
		{
			var properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(typeof(TEvDto)).ToArray();
			return properties.Select(property =>
			{
				var parameterAttribute = property.GetCustomAttribute<ParameterAttribute>(true);

				return new Topic
				{
					Name = property.Name,
					Alias = parameterAttribute.Name
				};
			}).ToList();
		}
	}
}