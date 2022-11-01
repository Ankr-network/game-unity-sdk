using System;
using System.Collections.Generic;
using System.Linq;
using AnkrSDK.Aptos.Constants;
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

		private IEnumerable<byte[]> ABIsToBytes(string[] ABIs)
		{
			foreach (var abi in ABIs)
			{
				yield return abi.HexToByteArray();
			}
		}
	}
}