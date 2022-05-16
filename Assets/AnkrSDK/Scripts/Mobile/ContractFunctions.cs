using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using Nethereum.Contracts;
using Nethereum.Web3;

namespace AnkrSDK.Mobile
{
	public class ContractFunctions : IContractFunctions
	{
		private readonly IWeb3 _web3Provider;

		public ContractFunctions(IWeb3 web3Provider)
		{
			_web3Provider = web3Provider;
		}

		public Task<TReturnType> GetContractData<TFieldData, TReturnType>(string contractAddress,
			TFieldData requestData = null) where TFieldData : FunctionMessage, new()
		{
			var contract = _web3Provider.Eth.GetContractHandler(contractAddress);
			return contract.QueryAsync<TFieldData, TReturnType>(requestData);
		}
	}
}