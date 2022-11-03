using System.Linq;
using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class Script : SerializableAbiPart<Script>
	{
		public readonly byte[] Code;
		public readonly TypeTag[] TypesArgs;
		public readonly TransactionArgument[] Args;

		public Script(byte[] code, TypeTag[] typesArgs, TransactionArgument[] args)
		{
			Code = code;
			TypesArgs = typesArgs;
			Args = args;
		}
		
		public void Serialize(Serializer serializer)
		{
			serializer.SerializeBytes(Code);
			serializer.SerializeVector(TypesArgs);
			serializer.SerializeVector(Args);
		}

		public static Script Deserialize(Deserializer deserializer)
		{
			var code = deserializer.DeserializeBytes();
			var typesArgs = deserializer.DeserializeVector(TypeTag.Deserialize);
			var args = deserializer.DeserializeVector(TransactionArgument.Deserialize);

			return new Script(code, typesArgs.ToArray(), args.ToArray());
		}
	}
}