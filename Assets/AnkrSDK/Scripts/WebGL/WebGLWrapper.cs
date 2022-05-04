using System;
using System.Collections.Generic;
using System.Linq;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.WebGL
{
	public class WebGLWrapper
	{
		private static WebGLWrapper instance;
		
		private WebGLCommunicationProtocol _protocol;

		protected WebGLWrapper()
		{
			_protocol = new WebGLCommunicationProtocol();
			_protocol.Connect().Forget();
		}

		public static WebGLWrapper Instance()
		{
			if (instance == null)
			{
				instance = new WebGLWrapper();
			}

			return instance;
		}
    
		public async UniTask<string> SendTransaction(TransactionData transaction)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.SendTransaction(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}
			
			throw new Exception(answer.payload);
		}
		
		public async UniTask<HexBigInteger> EstimateGas(TransactionData transaction)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.EstimateGas(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return new HexBigInteger(answer.payload);
			}
			
			throw new Exception(answer.payload);
		}

		public async UniTask<string> Sign(DataSignaturePropsDTO signProps)
		{
			var id = _protocol.GenerateId();

			WebGLInterlayer.SignMessage(id, JsonConvert.SerializeObject(signProps));
        
			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}
			
			throw new Exception(answer.payload);
		}
		
		public async UniTask<string> GetDefaultAccount()
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetAddresses(id);
        
			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var addresses = JsonConvert.DeserializeObject<string[]>(answer.payload);
				return addresses.First();
			}

			throw new Exception(answer.payload);
		}
		
		public async UniTask<Transaction> GetTransaction(string transactionHash)
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetTransaction(id, transactionHash);
        
			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var transactionData = JsonConvert.DeserializeObject<Transaction>(answer.payload);
				return transactionData;
			}

			throw new Exception(answer.payload);
		}
		
		public async UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetTransactionReceipt(id, transactionHash);
        
			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return JsonConvert.DeserializeObject<TransactionReceipt>(answer.payload);
			}

			throw new Exception(answer.payload);
		}
	}
}