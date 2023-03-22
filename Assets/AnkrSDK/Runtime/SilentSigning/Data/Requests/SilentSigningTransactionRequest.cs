using AnkrSDK.Core.Implementation;
using AnkrSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace AnkrSDK.SilentSigning.Data.Requests
{
	public class SilentSigningTransactionRequest : JsonRpcRequest
	{
		[JsonProperty("params")] private SilentTransactionData[] _parameters;

		[JsonIgnore] public SilentTransactionData[] Parameters => _parameters;

		public SilentSigningTransactionRequest(SilentTransactionData transactionData)
		{
			Method = "wallet_silentSendTransaction";
			_parameters = new[]
			{
				transactionData
			};
		}
	}
}