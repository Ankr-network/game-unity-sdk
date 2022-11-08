using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TypeTagStruct : TypeTag
	{
		public StructTag Value { get; private set; }

		public TypeTagStruct(StructTag value)
		{
			Value = value;
		}
		
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Struct);
			this.Value.Serialize(serializer);
		}

		public static TypeTagStruct Load(Deserializer deserializer)
		{
			var value = StructTag.Deserialize(deserializer);
			return new TypeTagStruct(value);
		}
	}
}