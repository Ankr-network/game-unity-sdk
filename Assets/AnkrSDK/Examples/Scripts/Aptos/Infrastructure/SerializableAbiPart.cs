using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Infrastructure
{
	public interface SerializableAbiPart
	{
		void Serialize(Serializer serializer);
		
		static SerializableAbiPart Deserialize(Deserializer deserializer)
		{
			throw new System.NotImplementedException();
		}
	}
}