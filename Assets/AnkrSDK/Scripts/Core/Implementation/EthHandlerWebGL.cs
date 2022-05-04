#if UNITY_WEBGL
using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.WebGL;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using AnkrSDK.Core.Utils;
using AnkrSDK.WebGL.Extensions;

namespace AnkrSDK.Core.Implementation
{
	public class EthHandlerWebGL : IEthHandler
	{
		private readonly WebGLWrapper _webGlWrapper;

		public EthHandlerWebGL()
		{
			_webGlWrapper = WebGLWrapper.Instance();
		}
		
		public Task<string> GetDefaultAccount()
		{
			return _webGlWrapper.GetDefaultAccount();
		}

		public async Task<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			return await _webGlWrapper.GetTransactionReceipt(transactionHash);
		}

		public async Task<Transaction> GetTransaction(string transactionReceipt)
		{
			return await _webGlWrapper.GetTransaction(transactionReceipt);
		}

		public async Task<HexBigInteger> EstimateGas(TransactionInput transactionInput)
		{
			return await _webGlWrapper.EstimateGas(transactionInput.ToTransactionData());
		}
		
		public async Task<HexBigInteger> EstimateGas(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		)
		{
			var transactionData = new WebGL.DTO.TransactionData
			{
				from = from,
				to = to,
				data = data,
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null,
				nonce = nonce
			};
			
			return await _webGlWrapper.EstimateGas(transactionData);
		}
	}
}
#endif