using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TransactionScriptABI : SerializableAbiPart
	{
		public string Name { get; private set; }
		public string Doc { get; private set; }
		public byte[] Code { get; private set; }

		public void Serialize(Serializer serializer)
		{
			throw new System.NotImplementedException();
		}

		public void Deserialize(Deserializer deserializer)
		{
			throw new System.NotImplementedException();
		}
	}
}