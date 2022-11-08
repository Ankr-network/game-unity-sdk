using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TypeTagBool : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Bool);
		}

		public static TypeTag Load(Deserializer deserializer)
		{
			return new TypeTagBool();
		}
	}
}