using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TypeTagAddress : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Address);
		}

		public static TypeTagAddress Load(Deserializer deserializer)
		{
			return new TypeTagAddress();
		}
	}
}