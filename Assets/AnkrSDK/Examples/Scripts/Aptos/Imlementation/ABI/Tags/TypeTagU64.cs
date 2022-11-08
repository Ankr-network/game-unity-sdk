using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TypeTagU64 : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.U64);
		}

		public static TypeTagU64 Load(Deserializer deserializer)
		{
			return new TypeTagU64();
		}
	}
}