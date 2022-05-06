using System.Threading.Tasks;
using Nethereum.Contracts;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IContractFunctions
	{
		Task<TReturnType> GetContractData<TFieldData, TReturnType>(string contractAddress,
			TFieldData requestData = null) where TFieldData : FunctionMessage, new();
	}
}