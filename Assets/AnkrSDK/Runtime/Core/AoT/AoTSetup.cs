using System.Numerics;
using AnkrSDK.Data.ContractMessages.ERC721;
using AnkrSDK.Data.ContractMessages.ERC721.RentableExtension;
using JetBrains.Annotations;
using Nethereum.Web3;

namespace AnkrSDK.Core.AoT
{
	public static class AoTSetup
	{
		[UsedImplicitly]
		private static void SetupAoT()
		{
			var web3 = new Web3();
			var contractHandler = web3.Eth.GetContractHandler("");
			contractHandler.QueryAsync<Data.ContractMessages.ERC721.BalanceOfMessage, BigInteger>();
			contractHandler.QueryAsync<Data.ContractMessages.ERC1155.BalanceOfMessage, BigInteger>();
			contractHandler.QueryAsync<TotalSupplyMessage, BigInteger>();
			contractHandler.QueryAsync<TokenOfOwnerByIndexMessage, BigInteger>();
			contractHandler.QueryAsync<TokenByIndexMessage, BigInteger>();
			contractHandler.QueryAsync<TokenURIMessage, string>();
			contractHandler.QueryAsync<OwnerOfMessage, string>();
			contractHandler.QueryAsync<NameMessage, string>();
			contractHandler.QueryAsync<SymbolMessage, string>();
			contractHandler.QueryAsync<PrincipalOwnerMessage, string>();
			contractHandler.QueryAsync<IsApprovedForAllMessage, bool>();
			contractHandler.QueryAsync<GetApprovedMessage, bool>();
			contractHandler.QueryAsync<IsRentedMessage, bool>();
		}
	}
}