using System.Collections.Generic;
using System.Threading.Tasks;
using MirageSDK.Core.Implementation;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.Core.Infrastructure
{
	public interface IContract
	{
		Task<string> CallMethod(string methodName, object[] arguments, string gas = null);
		Task<Transaction> GetTransactionInfo(string receipt);

		Task<string> SendTransaction(
			string to,
			string data = null,
			string value = null,
			string gas = null);

		Task<List<EventLog<TEvDto>>> GetAllChanges<TEvDto>(EventFilterData evFilter = null) where TEvDto : IEventDTO, new();

		Task<TReturnType> GetData<TFieldData, TReturnType>(TFieldData requestData = null)
			where TFieldData : FunctionMessage, new();
	}
}