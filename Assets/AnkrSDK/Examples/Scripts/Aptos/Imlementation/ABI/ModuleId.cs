using System;
using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class ModuleId : SerializableAbiPart<ModuleId>
	{
		private const string Delimiter = "::";
		
		public readonly byte[] Address;
		public readonly string Name;

		public ModuleId(byte[] address, string name)
		{
			Address = address;
			Name = name;
		}
		
		public static ModuleId FromString(string moduleId)
		{
			var parts = moduleId.Split(Delimiter);
			if (parts.Length != 2)
			{
				throw new Exception("Invalid module id.");
			}

			return new ModuleId(parts[0].HexToByteArray(), parts[1]);
		}
		
		public void Serialize(Serializer serializer)
		{
			serializer.SerializeBytes(Address);
			serializer.SerializeString(Name);
		}

		public static ModuleId Deserialize(Deserializer deserializer)
		{
			var address = deserializer.DeserializeFixedBytes((int)TypeLength.Address);
			var name = deserializer.DeserializeString();

			return new ModuleId(address, name);
		}
	}
}