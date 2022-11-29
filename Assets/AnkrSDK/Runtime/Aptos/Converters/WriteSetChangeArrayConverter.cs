using System;
using System.Collections.Generic;
using AnkrSDK.Aptos.Constants;
using AnkrSDK.Aptos.DTO;
using AnkrSDK.Aptos.DTO.ChageTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnkrSDK.Aptos.Converters
{
	public class WriteSetChangeArrayConverter : JsonConverter<WriteSetChange[]>
	{
		private const string TypeFieldName = "type";

		public override void WriteJson(JsonWriter writer, WriteSetChange[] value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		public override WriteSetChange[] ReadJson(
			JsonReader reader,
			Type objectType,
			WriteSetChange[] existingValue,
			bool hasExistingValue,
			JsonSerializer serializer
		)
		{
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw new JsonException("Object is not array.");
			}

			reader.Read();

			var list = new List<WriteSetChange>();

			while (reader.TokenType != JsonToken.EndArray)
			{
				var obj = Parse(reader, serializer);
				list.Add(obj);
				reader.Read();
			}

			return list.ToArray();
		}

		private WriteSetChange Parse(JsonReader reader, JsonSerializer serializer)
		{
			var jsonObj = JObject.Load(reader);
			var type = jsonObj[TypeFieldName]?.Value<string>();

			if (type == null)
			{
				throw new JsonException($"Field \"{TypeFieldName}\" doesn't exist in json object.");
			}

			switch (type)
			{
				case WriteSetChangeTypes.DeleteModule:
					return jsonObj.ToObject<DeleteModule>(serializer);
				case WriteSetChangeTypes.DeleteResource:
					return jsonObj.ToObject<DeleteResource>(serializer);
				case WriteSetChangeTypes.DeleteTableItem:
					return jsonObj.ToObject<DeleteTableItem>(serializer);
				case WriteSetChangeTypes.WriteModule:
					return jsonObj.ToObject<WriteModule>(serializer);
				case WriteSetChangeTypes.WriteResource:
					return jsonObj.ToObject<WriteResource>(serializer);
				case WriteSetChangeTypes.WriteTableItem:
					return jsonObj.ToObject<WriteTableItem>(serializer);
				default:
					throw new JsonException($"Converter to type \"{type}\" doesn't allow.");
			}
		}
	}
}