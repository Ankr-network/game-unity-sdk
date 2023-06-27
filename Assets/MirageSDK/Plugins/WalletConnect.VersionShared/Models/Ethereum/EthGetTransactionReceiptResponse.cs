using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthGetTransactionReceiptResponse : JsonRpcResponse
	{
		[JsonProperty]
		public TransactionReceipt result;

		[JsonIgnore]
		public TransactionReceipt Result => result;
	}
}