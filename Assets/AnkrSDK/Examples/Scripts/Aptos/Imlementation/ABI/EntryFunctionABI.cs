using System.Linq;
using AnkrSDK.Aptos.Imlementation.ABI;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class EntryFunctionABI : ScriptAbi
	{
		public readonly string Name;
		public readonly ModuleId ModuleId;
		public readonly string Doc;
		public readonly TypeArgumentABI[] TypesArgs;
		public readonly ArgumentABI[] Args;

		public EntryFunctionABI(string name, ModuleId moduleId, string doc, TypeArgumentABI[] typesArgs, ArgumentABI[] args)
		{
			Name = name;
			ModuleId = moduleId;
			Doc = doc;
			TypesArgs = typesArgs;
			Args = args;
		}
		
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(1);
			serializer.SerializeString(Name);
			ModuleId.Serialize(serializer);
			serializer.SerializeString(Doc);
			serializer.SerializeVector(TypesArgs);
			serializer.SerializeVector(Args);
		}

		public static EntryFunctionABI Load(Deserializer deserializer)
		{
			var name = deserializer.DeserializeString();
			var moduleName = ABI.ModuleId.Deserialize(deserializer);
			var doc = deserializer.DeserializeString();
			var typesArgs = deserializer.DeserializeVector(TypeArgumentABI.Deserialize);
			var args = deserializer.DeserializeVector(ArgumentABI.Deserialize);

			return new EntryFunctionABI(name, moduleName, doc, typesArgs.ToArray(), args.ToArray());
		}
	}
}