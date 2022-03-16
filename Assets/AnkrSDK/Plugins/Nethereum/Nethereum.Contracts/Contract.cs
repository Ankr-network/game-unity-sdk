using System;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Filters;
using System.Linq;

namespace Nethereum.Contracts
{
    public class Contract
    {
        public Contract(EthApiService eth, string abi, string contractAddress)
        {
            Eth = eth;
            ContractBuilder = new ContractBuilder(abi, contractAddress);
        }

        public Contract(EthApiService eth, Type contractMessageType, string contractAddress)
        {
            Eth = eth;
            ContractBuilder = new ContractBuilder(contractMessageType, contractAddress);
        }

        public Contract(EthApiService eth, Type[] contractMessagesTypes, string contractAddress)
        {
            Eth = eth;
            ContractBuilder = new ContractBuilder(contractMessagesTypes, contractAddress);
        }

        private IEthNewFilter EthNewFilter => Eth.Filters.NewFilter;

        public ContractBuilder ContractBuilder { get; set; }

        public string Address => ContractBuilder.Address;

        public EthApiService Eth { get; }

        public Task<HexBigInteger> CreateFilterAsync()
        {
            var ethFilterInput = GetDefaultFilterInput();
            return EthNewFilter.SendRequestAsync(ethFilterInput);
        }

        public NewFilterInput GetDefaultFilterInput(BlockParameter fromBlock = null, BlockParameter toBlock = null)
        {
            return ContractBuilder.GetDefaultFilterInput(fromBlock, toBlock);
        }

        public Event GetEvent(string name)
        {
            return new Event(this, ContractBuilder.GetEventAbi(name));
        }

        public Error GetError(string name)
        {
            return new Error(ContractBuilder.GetErrorAbi(name));
        }

        public Error FindError(string exceptionEncodedData)
        {
            var errors = ContractBuilder.ContractABI.Errors;
            var errorAbi =  errors.FirstOrDefault(x => x.IsExceptionEncodedDataForError(exceptionEncodedData));
            if (errorAbi == null) return null;
            return new Error(errorAbi);
        }

        public Event GetEventBySignature(string signature)
        {
            return new Event(this, ContractBuilder.GetEventAbiBySignature(signature));
        }

        public Event<T> GetEvent<T>() where T: IEventDTO, new()
        {
            return new Event<T>(this);
        }

        public Function<TFunction> GetFunction<TFunction>()
        {
            return new Function<TFunction>(this, GetFunctionBuilder<TFunction>());
        }

        public Function GetFunction(string name)
        {
            return new Function(this, GetFunctionBuilder(name));
        }

        public Function GetFunctionBySignature(string signature)
        {
            return new Function(this, GetFunctionBuilderBySignature(signature));
        }

        private FunctionBuilder GetFunctionBuilder(string name)
        {
            return ContractBuilder.GetFunctionBuilder(name);
        }

        private FunctionBuilder GetFunctionBuilderBySignature(string signature)
        {
            return ContractBuilder.GetFunctionBuilderBySignature(signature);
        }

        private FunctionBuilder<TFunctionInput> GetFunctionBuilder<TFunctionInput>()
        {
            return ContractBuilder.GetFunctionBuilder<TFunctionInput>();
        }
    }
}