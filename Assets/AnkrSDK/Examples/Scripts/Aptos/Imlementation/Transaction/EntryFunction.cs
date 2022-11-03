using System.Linq;
using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class EntryFunction : SerializableAbiPart<EntryFunction>
	{
		public readonly ModuleId ModuleName;
		public readonly string FunctionName;
		public readonly TypeTag[] TypesArgs;
		public readonly byte[][] Args;

		public EntryFunction(ModuleId moduleName, string functionName, TypeTag[] typesArgs, byte[][] args)
		{
			ModuleName = moduleName;
			FunctionName = functionName;
			TypesArgs = typesArgs;
			Args = args;
		}

		public EntryFunction(string moduleName, string functionName, TypeTag[] typesArgs, byte[][] args) :
			this(ModuleId.FromString(moduleName), functionName, typesArgs, args)
		{
		}

		public void Serialize(Serializer serializer)
		{
			ModuleName.Serialize(serializer);
			serializer.SerializeString(FunctionName);
			serializer.SerializeVector(TypesArgs);
			
			serializer.SerializeUInt32AsUleb128((uint)Args.Length);
			foreach (var argBytes in Args)
			{
				serializer.SerializeBytes(argBytes);
			}
		}

		public static EntryFunction Deserialize(Deserializer deserializer)
		{
			var moduleName = ModuleId.Deserialize(deserializer);
			var functionName = deserializer.DeserializeString();
			var typesArgs = deserializer.DeserializeVector(TypeTag.Deserialize);

			var length = deserializer.DeserializeUleb128AsUint32();
			var args = new byte[length][];
			for (int i = 0; i < length; i++)
			{
				args[i] = deserializer.DeserializeBytes();
			}

			return new EntryFunction(moduleName, functionName, typesArgs.ToArray(), args);
		}
	}
}