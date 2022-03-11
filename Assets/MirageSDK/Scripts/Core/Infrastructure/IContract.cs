using System.Collections.Generic;
using System.Threading.Tasks;
using MirageSDK.Core.Data;
using MirageSDK.Core.Events;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Core.Infrastructure
{
	public interface IContract
	{
		Task<string> CallMethod(string methodName, object[] arguments = null, string gas = null, string gasPrice = null,
			string nonce = null);

		Task Web3SendMethod(string methodName, object[] arguments, EventController evController = null,
			string gas = null, string gasPrice = null, string nonce = null);

		Task<List<EventLog<TEvDto>>> GetAllChanges<TEvDto>(EventFilterData evFilter = null)
			where TEvDto : IEventDTO, new();

		Task<TReturnType> GetData<TFieldData, TReturnType>(TFieldData requestData = null)
			where TFieldData : FunctionMessage, new();
	}
}