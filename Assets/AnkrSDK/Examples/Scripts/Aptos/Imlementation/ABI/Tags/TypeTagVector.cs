using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TypeTagVector : TypeTag
	{
		public TypeTag Value { get; private set; }

		public TypeTagVector(TypeTag value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Vector);
			Value.Serialize(serializer);
		}

		public static TypeTagVector Load(Deserializer deserializer)
		{
			var value = Deserialize(deserializer);
			return new TypeTagVector(value);
		}
	}
}