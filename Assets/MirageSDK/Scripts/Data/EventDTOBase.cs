using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Newtonsoft.Json;

namespace MirageSDK.Data
{
	public class EventDTOBase : IEventDTO
	{
		public override string ToString()
		{
			var type = GetType();
			var stringBuilder = new StringBuilder();
			foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				// Check if the property has a ParameterAttribute
				var hasAttribute = property.IsDefined(typeof(ParameterAttribute), false);
				if (hasAttribute)
				{
					string valueString;

					var value = property.GetValue(this);
					var propertyType = property.PropertyType;

					if (propertyType == typeof(byte))
					{
						valueString = $"0x{((byte)value):X2}";
					}
					else if (propertyType == typeof(byte[]))
					{
						var byteArray = (byte[])value;
						valueString = BitConverter.ToString(byteArray).Replace("-", "0x");
					}
					else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
					{
						var list = (System.Collections.IList)value;
						var elements = new List<string>();
						foreach (var element in list)
						{
							elements.Add(element.ToString());
						}
						valueString = string.Join(", ", elements);
					}
					else if (propertyType.IsClass || propertyType.IsValueType)
					{
						var serializableAttribute = propertyType.GetCustomAttribute(typeof(SerializableAttribute), false);
						if (serializableAttribute != null)
						{
							valueString = JsonConvert.SerializeObject(value, Formatting.None);
						}
						else
						{
							valueString = value.ToString();
						}
					}
					else
					{
						valueString = value.ToString();
					}

					stringBuilder.Append($"{property.Name}: {valueString}\n");
				}
			}

			return stringBuilder.ToString();
		}
	}
}