using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IContractTransactions
	{
		Task<string> GetContractData(AnkrSDK.WebGL.DTO.TransactionData transactionData);
	}
}