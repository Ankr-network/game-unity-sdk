using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TypeTagSigner : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Signer);
		}

		public static TypeTagSigner Load(Deserializer deserializer)
		{
			return new TypeTagSigner();
		}
	}
}