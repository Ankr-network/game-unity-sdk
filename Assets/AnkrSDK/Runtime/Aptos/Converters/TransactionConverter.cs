using System;
using AnkrSDK.Aptos.Constants;
using AnkrSDK.Aptos.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AnkrSDK.Aptos.Converters
{
	public class TransactionConverter : JsonConverter<TypedTransaction>
	{
		private const string TypeFieldName = "type";

		public override void WriteJson(JsonWriter writer, TypedTransaction value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		public override TypedTransaction ReadJson(JsonReader reader, Type objectType,
			TypedTransaction existingValue,
			bool hasExistingValue, JsonSerializer serializer)
		{
			var jsonObj = JObject.Load(reader);
			var type = jsonObj[TypeFieldName]?.Value<string>();

			if (type == null)
			{
				throw new JsonException($"Field \"{TypeFieldName}\" doesn't exist in json object.");
			}

			Debug.Log(type);
			
			switch (type)
			{
				case TransactionTypes.UserTransaction:
					return jsonObj.ToObject<Transaction_UserTransaction>(serializer);
				case TransactionTypes.GenesisTransaction:
					return jsonObj.ToObject<GenesisTransaction>(serializer);
				case TransactionTypes.PendingTransaction:
					return jsonObj.ToObject<Transaction_PendingTransaction>(serializer);
				case TransactionTypes.BlockMetadataTransaction:
					return jsonObj.ToObject<BlockMetadataTransaction>(serializer);
				case TransactionTypes.StateCheckpointTransaction:
					return jsonObj.ToObject<StateCheckpointTransaction>(serializer);
				default:
					throw new JsonException($"Converter for transaction type \"{type}\" doesn't exist.");
			}
		}
	}
}