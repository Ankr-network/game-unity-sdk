using System.Numerics;
using AnkrSDK.Data.ContractMessages.ERC721;
using AnkrSDK.Data.ContractMessages.ERC721.RentableExtension;
using JetBrains.Annotations;
using Nethereum.Web3;

namespace AnkrSDK.Provider
{
	public static class AoTSetup
	{
		[UsedImplicitly]
		private static void SetupAoT()
		{
			var web3 = new Web3();

		#if (UNITY_WEBGL && !UNITY_EDITOR)
			var webGlWrapper = new WebGL.WebGLWrapper();
			var contractFunctions = new WebGL.Implementation.ContractFunctionsWebGL(webGlWrapper);
		#else
			var contractFunctions = new Mobile.ContractFunctions(web3);
		#endif
			contractFunctions.GetContractData<Data.ContractMessages.ERC721.BalanceOfMessage, BigInteger>("");
			contractFunctions.GetContractData<Data.ContractMessages.ERC1155.BalanceOfMessage, BigInteger>("");

			contractFunctions.GetContractData<TotalSupplyMessage, BigInteger>("");
			contractFunctions.GetContractData<TokenOfOwnerByIndexMessage, BigInteger>("");
			contractFunctions.GetContractData<TokenByIndexMessage, BigInteger>("");
			contractFunctions.GetContractData<TokenURIMessage, string>("");
			contractFunctions.GetContractData<OwnerOfMessage, string>("");
			contractFunctions.GetContractData<NameMessage, string>("");
			contractFunctions.GetContractData<SymbolMessage, string>("");
			contractFunctions.GetContractData<PrincipalOwnerMessage, string>("");
			contractFunctions.GetContractData<IsApprovedForAllMessage, bool>("");
			contractFunctions.GetContractData<GetApprovedMessage, bool>("");
			contractFunctions.GetContractData<IsRentedMessage, bool>("");
		}
	}
}