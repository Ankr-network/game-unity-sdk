using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Web3;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.Unity;
using Web3Unity.Scripts.Library;

public class Contract
{
	public string Abi;
	public string Address;
	
	private IWeb3 _web3;
	private IClient _client;

	public Contract(IWeb3 web3, IClient client, string address, string abi)
	{
		_web3 = web3;
		_client = client;
		Abi = abi;
		Address = address;
	}

	public Task<ReturnType> GetData<FieldData, ReturnType>(FieldData requestData = null) where FieldData : FunctionMessage, new()
	{
		var Contract = _web3.Eth.GetContractHandler(Address);
		return Contract.QueryAsync<FieldData, ReturnType>(requestData);
	}

	public Task<List<EventLog<EvDTO>>> GetAllChanges<EvDTO>() where EvDTO : IEventDTO, new()
	{
		var eventHandler = _web3.Eth.GetEvent<EvDTO>(Address);
		
		var filterAllTransferEventsForContract = eventHandler.CreateFilterInput();
		
		return eventHandler.GetAllChangesAsync(filterAllTransferEventsForContract);
	}
	
	public Task<string> CallMethod(string methodName, object[] arguments, string gas = null)
	{	
		TransactionInput raw = _web3.Eth.GetContract(Abi, Address)
			.GetFunction(methodName)
			.CreateTransactionInput(WalletConnect.ActiveSession.Accounts[0], arguments);		

		return SendTransaction(Address, raw.Data, null, gas);
	}
	
	public Task<Transaction> GetTransactionInfo(string receipt)
	{
		EthGetTransactionByHash src = new EthGetTransactionByHash(_client);
		return src.SendRequestAsync(receipt);
	}
	
	public async Task<string> SendTransaction(string _to, string _data = null, string _value = null, string _gas = null)
	{	
		string address = WalletConnect.ActiveSession.Accounts[0];
		
		var transaction = new TransactionData()
		{
			from = address,
			to = _to
		};
		
		if (_data != null)
		{
			transaction.data = _data;
		}

		if (_value != null)
		{
			transaction.value = Utils.ConvertNumber(_value);
		}
		
		if (_gas != null)
		{
			transaction.gas = Utils.ConvertNumber(_gas);
		}

		return await WalletConnect.ActiveSession.EthSendTransaction(transaction);
	}
	
	public async Task<string> SendTransaction(TransactionData data)
	{	
		return await WalletConnect.ActiveSession.EthSendTransaction(data);
	}
}
