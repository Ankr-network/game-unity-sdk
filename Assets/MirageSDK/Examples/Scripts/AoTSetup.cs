using System.Numerics;
using MirageSDK.GameCharacterContract;
using JetBrains.Annotations;
using Nethereum.Web3;

namespace MirageSDK
{
	public static class AoTSetup
	{
		public static class AoTSetupExamples
		{
			[UsedImplicitly]
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