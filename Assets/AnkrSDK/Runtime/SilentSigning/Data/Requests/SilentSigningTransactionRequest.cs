using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using Newtonsoft.Json;

namespace AnkrSDK.SilentSigning.Data.Requests
{
	public class SilentSigningTransactionRequest : JsonRpcRequest
	{
		[JsonProperty("params")] private SilentTransactionData[] _parameters;
		public override string Method => "wallet_requestSilentSign";

		[JsonIgnore] public SilentTransactionData[] Parameters => _parameters;

		public SilentSigningTransactionRequest(SilentTransactionData transactionData)
		{
			_parameters = new[]
			{
				transactionData
			};
		}
	}
}