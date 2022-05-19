using AnkrSDK.WebGL.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Utils;

namespace AnkrSDK.WebGL.Implementation
{
	public class EthHandlerWebGL : IEthHandler
	{
		private readonly WebGLWrapper _webGlWrapper;

		public EthHandlerWebGL(WebGLWrapper webGlWrapper)
		{
			_webGlWrapper = webGlWrapper;
		}

		public Task<string> GetDefaultAccount()
		{
			return _webGlWrapper.GetDefaultAccount();
		}

		public Task<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			return _webGlWrapper.GetTransactionReceipt(transactionHash);
		}

		public Task<Transaction> GetTransaction(string transactionReceipt)
		{
			return _webGlWrapper.GetTransaction(transactionReceipt);
		}

		public Task<HexBigInteger> EstimateGas(TransactionInput transactionInput)
		{
			return _webGlWrapper.EstimateGas(transactionInput.ToTransactionData());
		}

		public Task<HexBigInteger> EstimateGas(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		)
		{
			var transactionData = new DTO.TransactionData
			{
				from = from,
				to = to,
				data = data,
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null,
				nonce = nonce
			};

			return _webGlWrapper.EstimateGas(transactionData);
		}

		public Task<string> Sign(string messageToSign, string address)
		{
			var props = new DTO.DataSignaturePropsDTO
			{
				address = address,
				message = messageToSign
			};

			return _webGlWrapper.Sign(props);
		}

		public Task<string> SendTransaction(string from, string to, string data = null, string value = null,
			string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var transactionData = new DTO.TransactionData
			{
				from = from,
				to = to,
				data = data,
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null,
				nonce = nonce
			};

			return _webGlWrapper.SendTransaction(transactionData);
		}
	}
}