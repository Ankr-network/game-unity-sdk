using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
    public class Identifier : SerializableAbiPart<Identifier>
    {
        public string Value { get; private set; }

        public Identifier(string value)
        {
            Value = value;
        }
        
        public void Serialize(Serializer serializer)
        {
            serializer.SerializeString(Value);
        }

        public static Identifier Deserialize(Deserializer deserializer)
        {
            var value = deserializer.DeserializeString();
            return new Identifier(value);
        }
    }
}