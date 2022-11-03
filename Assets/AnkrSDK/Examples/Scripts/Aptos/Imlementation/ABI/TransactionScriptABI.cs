using System.Linq;
using AnkrSDK.Aptos.Imlementation.ABI;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TransactionScriptABI : ScriptAbi
	{
		public readonly string Name;
		public readonly string Doc;
		public readonly byte[] Code;
		public readonly TypeArgumentABI[] TypesArgs;
		public readonly ArgumentABI[] Args;

		public TransactionScriptABI(string name, string doc, byte[] code, TypeArgumentABI[] typesArgs, ArgumentABI[] args)
		{
			Name = name;
			Doc = doc;
			Code = code;
			TypesArgs = typesArgs;
			Args = args;
		}
		
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((uint)ScriptAbiIndex.TransactionScript);
			serializer.SerializeString(Name);
			serializer.SerializeString(Doc);
			serializer.SerializeBytes(Code);
			serializer.SerializeVector(TypesArgs);
			serializer.SerializeVector(Args);
		}

		public static TransactionScriptABI Load(Deserializer deserializer)
		{
			var name = deserializer.DeserializeString();
			var doc = deserializer.DeserializeString();
			var code = deserializer.DeserializeBytes();
			var typesArgs = deserializer.DeserializeVector(TypeArgumentABI.Deserialize);
			var args = deserializer.DeserializeVector(ArgumentABI.Deserialize);

			return new TransactionScriptABI(name, doc, code, typesArgs.ToArray(), args.ToArray());
		}
	}
}