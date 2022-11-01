using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class ArgumentABI : SerializableAbiPart<ArgumentABI>
	{
		public string Name { get; private set; }
		public TypeTag Tag { get; private set; }

		public ArgumentABI(string name, TypeTag typeTag)
		{
			Name = name;
			Tag = typeTag;
		}
		
		public void Serialize(Serializer serializer)
		{
			serializer.SerializeString(Name);
			Tag.Serialize(serializer);
		}

		public static ArgumentABI Deserialize(Deserializer deserializer)
		{
			var name = deserializer.DeserializeString();
			var typeTag = TypeTag.Deserialize(deserializer);
			return new ArgumentABI(name, typeTag);
		}
	}
}