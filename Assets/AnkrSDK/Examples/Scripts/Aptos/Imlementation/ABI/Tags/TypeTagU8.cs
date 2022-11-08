using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TypeTagU8 : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.U8);
		}

		public static TypeTagU8 Load(Deserializer deserializer)
		{
			return new TypeTagU8();
		}
	}
}