using System;
using AnkrSDK.Aptos.Constants;
using AnkrSDK.Aptos.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AnkrSDK.Aptos.Converters
{
	public class TransactionSimpleConverter<T> : JsonConverter<T>
	{
		private const string TypeFieldName = "type";

		public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		public override T ReadJson(JsonReader reader, Type objectType,
			T existingValue,
			bool hasExistingValue, JsonSerializer serializer)
		{
			return serializer.Deserialize<T>(reader);
		}
	}
}