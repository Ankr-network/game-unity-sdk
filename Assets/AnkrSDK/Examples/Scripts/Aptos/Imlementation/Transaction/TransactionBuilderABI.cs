using System;
using System.Collections.Generic;
using System.Linq;
using AnkrSDK.Aptos.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class TransactionBuilderABI
	{
		private const string Delimiter = "::";
		
		private readonly Dictionary<string, ScriptAbi> _abiMap = new Dictionary<string, ScriptAbi>();

		public TransactionBuilderABI(string[] ABIs)
		{
			var abis = ABIsToBytes(ABIs).ToArray();

			foreach (var abi in abis)
			{
				var deserializer = new Deserializer(abi);
				var scriptAbi = ScriptAbi.Deserialize(deserializer);
				var name = "";
				if (scriptAbi is EntryFunctionABI)
				{
					var funcAbi = (EntryFunctionABI)scriptAbi;
					name = funcAbi.ModuleId.Address.ToHexCompact(true) + Delimiter + funcAbi.ModuleId.Name + Delimiter +
					       funcAbi.Name;
				}
				else
				{
					var funcAbi = (TransactionScriptABI)scriptAbi;
					name = funcAbi.Name;
				}

				if (_abiMap.ContainsKey(name))
				{
					throw new Exception("Found conflicting ABI interfaces");
				}
				
				_abiMap.Add(name, scriptAbi);
			}

			Debug.Log(JsonConvert.SerializeObject(_abiMap));
		}

		public static IEnumerable<byte[]> ToBCSArgs(ArgumentABI[] abiArgs, object[] args)
		{
			if (abiArgs.Length != args.Length)
			{
				throw new Exception("Wrong number of args provided.");
			}

			for (int i = 0; i < abiArgs.Length; i++)
			{
				var serializer = new Serializer();
				BuildUtils.SerializeArgs(args[i], abiArgs[i].Tag, serializer);
				yield return serializer.GetBytes();
			}
		}
		
		public static IEnumerable<TransactionArgument> ToTransactionArguments(ArgumentABI[] abiArgs, object[] args)
		{
			if (abiArgs.Length != args.Length)
			{
				throw new Exception("Wrong number of args provided.");
			}
			
			for (int i = 0; i < abiArgs.Length; i++)
			{
				yield return BuildUtils.ArgToTransactionArgument(args[i], abiArgs[i].Tag);
			}
		}

		public TransactionPayload BuildTransactionPayload(string func, string[] argsTypes, object[] args)
		{
			if (!_abiMap.ContainsKey(func))
			{
				throw new Exception($"Cannot find function: ${func}");
			}
			
			var typeTags = ParseTypeTags(argsTypes).ToArray();
			var scriptAbi = _abiMap[func];
			TransactionPayload payload;
			if (scriptAbi is EntryFunctionABI)
			{
				var funcAbi = scriptAbi as EntryFunctionABI;
				var bcsArgs = ToBCSArgs(funcAbi.Args, args).ToArray();
				var entryFunction = new EntryFunction(funcAbi.ModuleId, funcAbi.Name, typeTags, bcsArgs);
				return new TransactionPayloadEntryFunction(entryFunction);
			}
			else if (scriptAbi is TransactionScriptABI)
			{
				var funcAbi = scriptAbi as TransactionScriptABI;
				var scriptArgs = ToTransactionArguments(funcAbi.Args, args).ToArray();
				var script = new Script(funcAbi.Code, typeTags, scriptArgs);
				return new TransactionPayloadScript(script);
			}
			else
			{
				throw new Exception("Unknown ABI format.");
			}
		}

		private IEnumerable<TypeTag> ParseTypeTags(string[] argsTypes)
		{
			foreach (var argType in argsTypes)
			{
				yield return new TypeTagParser(argType).ParseTypeTag();
			}
		}

		private IEnumerable<byte[]> ABIsToBytes(string[] ABIs)
		{
			foreach (var abi in ABIs)
			{
				yield return abi.HexToByteArray();
			}
		}
	}
}