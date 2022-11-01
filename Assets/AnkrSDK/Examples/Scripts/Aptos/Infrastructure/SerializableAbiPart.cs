using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Infrastructure
{
	public interface SerializableAbiPart<T>
	{
		void Serialize(Serializer serializer);
		
		static T Deserialize(Deserializer deserializer)
		{
			throw new System.NotImplementedException();
		}
	}
}