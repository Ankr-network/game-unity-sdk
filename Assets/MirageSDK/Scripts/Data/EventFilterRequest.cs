using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.Data
{
	public class EventFilterRequest<TEvDTO>
	{
		private class Topic
		{
			public string Name { get; set; }
			public string Alias { get; set; }
			public List<object> Values { get; set; }
		}
		
		private readonly IEnumerable<Topic> _topics;
		
		public BlockParameter FromBlock { get; set; }
		public BlockParameter ToBlock { get; set; }

		public EventFilterRequest()
		{
			_topics = CollectTopics();
		}

		public void AddTopic(string name, object value)
		{
			var topic = FindTopicByName(name);
			if (topic != null)
			{
				topic.Values.Add(value);
			}
			else
			{
				throw new Exception("Topic name is not allowed");
			}
		}
		
		public void AddTopics(string name, IEnumerable<object> values)
		{
			var topic = FindTopicByName(name);
			if (topic != null)
			{
				topic.Values.AddRange(values);
			}
			else
			{
				throw new Exception("Topic name is not allowed");
			}
		}

		public object[][] AssembleTopics()
		{
			return _topics.Select(topic => topic.Values.Any() ? topic.Values.ToArray() : null).ToArray();
		}

		private Topic FindTopicByName(string name)
		{
			return _topics.FirstOrDefault(topic => topic.Name == name || topic.Alias == name);
		}

		private static IEnumerable<Topic> CollectTopics()
		{
			var properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(typeof(TEvDTO)).ToArray();
			return properties.Select(property =>
			{
				var parameterAttribute = property.GetCustomAttribute<ParameterAttribute>(true);

				return new Topic
				{
					Name = property.Name,
					Alias = parameterAttribute.Name,
					Values = new List<object>()
				};
			});
		}
	}
}