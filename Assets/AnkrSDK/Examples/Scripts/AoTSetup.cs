using System.Numerics;
using AnkrSDK.GameCharacterContract;
using Nethereum.Web3;

namespace AnkrSDK
{
	public static class AoTSetup
	{
		public static class AoTSetupExamples
		{
			private static void SetupAoT()
			{
				var web3 = new Web3();
				var contractHandler = web3.Eth.GetContractHandler("");
				contractHandler.QueryAsync<GetHatMessage, BigInteger>();
				contractHandler.QueryAsync<GetShoesMessage, BigInteger>();
			}
		}
	}
}