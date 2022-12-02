using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using Newtonsoft.Json;

namespace AnkrSDK.SilentSigning.Data.Requests
{
	public class SilentSigningTransactionRequest : JsonRpcRequest
	{
		[JsonProperty("params")] 
		private TransactionData[] _parameters;
		public override string Method => "wallet_requestSilentSign";

		[JsonIgnore]
		public TransactionData[] Parameters => _parameters;

		public SilentSigningTransactionRequest(params TransactionData[] transactionDatas)
		{
			_parameters = transactionDatas;
		}
	}
}