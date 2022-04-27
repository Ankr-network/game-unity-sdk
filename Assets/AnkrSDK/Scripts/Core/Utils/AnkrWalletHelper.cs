using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using AnkrSDK.WebGL;
using UnityEngine;

namespace AnkrSDK.Core.Utils
{
	public static class AnkrWalletHelper
	{
		public static async Task<EthResponse> SendTransaction(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		)
		{
			var transactionData = new TransactionData
			{
				from = from,
				to = to,
				data = data,
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null,
				nonce = nonce
			};

			#if !UNITY_WEBGL

			var request = new EthSendTransaction(transactionData);
			return WalletConnect.ActiveSession.Send<EthSendTransaction, EthResponse>(request);
	
			#else
			
			var interlayer = WebGLWrapper.Instance();
			var response = new EthResponse
			{
				result = await interlayer.SendTransaction(transactionData)
			};
			return response;
			
			#endif
		}
	}
}