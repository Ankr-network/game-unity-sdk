using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TypeTagU128 : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.U128);
		}

		public static TypeTagU128 Load(Deserializer deserializer)
		{
			return new TypeTagU128();
		}
	}
}