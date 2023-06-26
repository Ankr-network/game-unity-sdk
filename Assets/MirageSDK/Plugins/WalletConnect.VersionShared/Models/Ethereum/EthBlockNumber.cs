namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthBlockNumber : JsonRpcRequest
	{
		public EthBlockNumber()
		{
			this.Method = "eth_blockNumber";
		}
	}
}