using System;
using AnkrSDK.Aptos.Constants;
using AnkrSDK.Aptos.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnkrSDK.Aptos.Converters
{
	public class TransactionPayloadConverter : JsonConverter<TransactionPayload>
	{
		private const string TypeFieldName = "type";

		public override void WriteJson(JsonWriter writer, TransactionPayload value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		public override TransactionPayload ReadJson(
			JsonReader reader,
			Type objectType,
			TransactionPayload existingValue,
			bool hasExistingValue,
			JsonSerializer serializer
		)
		{
			var jsonObj = JObject.Load(reader);
			var type = jsonObj[TypeFieldName]?.Value<string>();

			if (type == null)
			{
				throw new JsonException($"Field \"{TypeFieldName}\" doesn't exist in json object.");
			}

			switch (type)
			{
				case TransactionPayloadTypes.EntryFunction:
					return jsonObj.ToObject<EntryFunctionPayload>(serializer);
				case TransactionPayloadTypes.Script:
					return jsonObj.ToObject<ScriptPayload>(serializer);
				case TransactionPayloadTypes.ModuleBundle:
					throw new NotImplementedException();
				default:
					throw new JsonException($"Converter for payload type {type} doesn't exist.");
			}
		}
	}
}