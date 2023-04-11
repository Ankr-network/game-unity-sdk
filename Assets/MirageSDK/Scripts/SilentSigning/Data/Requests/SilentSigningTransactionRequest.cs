using MirageSDK.Core.Implementation;
using MirageSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace MirageSDK.SilentSigning.Data.Requests
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