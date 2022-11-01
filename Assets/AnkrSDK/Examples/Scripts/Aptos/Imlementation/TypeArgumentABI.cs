namespace AnkrSDK.Aptos.Utils
{
	public class TypeArgumentABI
	{
		public string Name { get; private set; }

		public TypeArgumentABI(string name)
		{
			Name = name;
		}
		
		public void Serialize(Serializer serializer) {
			serializer.SerializeString(Name);
		}
		
		public static TypeArgumentABI Deserialize(Deserializer deserializer) {
			var name = deserializer.DeserializeString();
			return new TypeArgumentABI(name);
		}
	}
}