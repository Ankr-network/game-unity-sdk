using System.Collections.Generic;
using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.CommonUtils;
using MirageSDK.Core.Infrastructure;
using Nethereum.Contracts;
using UnityEngine;

namespace MirageSDK.UseCases.MirageNFT
{
	public class MirageNftContractReader
	{
		private static readonly BigInteger[] ZeroBalanceResponse = { -2 };
		
		private readonly IContract _contract;
		private readonly IEthHandler _ethHandler;

		public MirageNftContractReader(IContract contract, IEthHandler ethHandler)
		{
			_contract = contract;
			_ethHandler = ethHandler;
		}

		public async UniTask<IReadOnlyList<BigInteger>> GetOwnedNftIds()
		{
			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var balance = (int)await GetNftBalance();
			
			if (balance == 0)
			{
				Debug.Log($"MirageNFTContractReader: for user {defaultAccount} UserBalance = 0");
				return ZeroBalanceResponse;
			}
			
			var ownedEpochsArray = new BigInteger[balance];
			for (var i = 0; i < balance; i++)
			{
				var tokenId = await _contract.TokenOfOwnerByIndex(defaultAccount, i);
				ownedEpochsArray[i] = tokenId;
			}
			
			Debug.Log($"Address {defaultAccount} owns {balance} epochs");

			return ownedEpochsArray;
		}

		public UniTask<string> GetUriFromNftId(BigInteger id)
		{
			return _contract.TokenURI(id);
		}
		
		private async UniTask<BigInteger> GetNftBalance()
		{
			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var balance = await _contract.BalanceOf(defaultAccount);
			return balance;
		}
	}
}