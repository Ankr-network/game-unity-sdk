﻿using System;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using System.Numerics;
using System.Threading;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Fee1559Suggestions;
using Nethereum.RPC.TransactionReceipts;
using Nethereum.RPC.TransactionTypes;

namespace Nethereum.RPC.TransactionManagers
{
    public abstract class TransactionManagerBase : ITransactionManager
    {
        public virtual IClient Client { get; set; }
        public BigInteger DefaultGasPrice { get; set; } = -1; // Setting the default gas price to -1 as a flag
        public abstract BigInteger DefaultGas { get; set; }
        public IAccount Account { get; protected set; }
        public bool UseLegacyAsDefault { get; set; } = false;

        public bool IsTransactionToBeSendAsEIP1559(TransactionInput transaction)
        {
            return (!UseLegacyAsDefault && transaction.GasPrice == null) || (transaction.MaxPriorityFeePerGas != null) || (transaction.Type != null && transaction.Type.Value == TransactionType.EIP1559.AsByte());
        }

#if !DOTNET35
        public BigInteger DefaultMaxPriorityFeePerGas { get; set; } = SimpleFeeSuggestionStrategy.DEFAULT_MAX_PRIORITY_FEE_PER_GAS;

        private IFee1559SuggestionStrategy _fee1559SuggestionStrategy;
        public IFee1559SuggestionStrategy Fee1559SuggestionStrategy
        {
            get
            {
                if (_fee1559SuggestionStrategy == null)
                    _fee1559SuggestionStrategy = new SimpleFeeSuggestionStrategy(Client);
                return _fee1559SuggestionStrategy;
            }
            set => _fee1559SuggestionStrategy = value;
        }

        public abstract Task<string> SignTransactionAsync(TransactionInput transaction);

        protected async Task SetTransactionFeesOrPricingAsync(TransactionInput transaction)
        {
            if (IsTransactionToBeSendAsEIP1559(transaction))
            {
                transaction.Type = new HexBigInteger(TransactionType.EIP1559.AsByte());
                if (transaction.MaxPriorityFeePerGas != null)
                {
                    if (transaction.MaxFeePerGas == null)
                    {
                        var fee1559 = await CalculateFee1559Async(transaction.MaxPriorityFeePerGas.Value).ConfigureAwait(false);
                        transaction.MaxFeePerGas = new HexBigInteger(fee1559.MaxFeePerGas.Value);
                    }
                }
                else
                {
                    var fee1559 = await CalculateFee1559Async().ConfigureAwait(false);
                    if (transaction.MaxFeePerGas == null)
                    {
                        transaction.MaxFeePerGas =
                            new HexBigInteger(fee1559.MaxFeePerGas.Value);

                        transaction.MaxPriorityFeePerGas =
                            new HexBigInteger(fee1559.MaxPriorityFeePerGas.Value);
                    }
                    else
                    {
                        if (transaction.MaxFeePerGas < fee1559.MaxPriorityFeePerGas)
                        {
                            transaction.MaxPriorityFeePerGas = transaction.MaxFeePerGas;
                        }
                        else
                        {
                            transaction.MaxPriorityFeePerGas = new HexBigInteger(fee1559.MaxPriorityFeePerGas.Value);
                        }
                    }
                }
            }
            else
            {
                if (transaction.GasPrice == null)
                {
                    var gasPrice = await GetGasPriceAsync(transaction).ConfigureAwait(false);
                    transaction.GasPrice = gasPrice;
                }
            }
        }

        public Task<string> SendRawTransactionAsync(string signedTransaction)
        {
            if (Client == null) throw new NullReferenceException("Client not configured");
            if (string.IsNullOrEmpty(signedTransaction)) throw new ArgumentNullException(nameof(signedTransaction));
            var ethSendRawTransaction = new EthSendRawTransaction(Client);
            return ethSendRawTransaction.SendRequestAsync(signedTransaction);
        }

        private ITransactionReceiptService _transactionReceiptService;
        public ITransactionReceiptService TransactionReceiptService {
            get
            {
                if (_transactionReceiptService == null) return TransactionReceiptServiceFactory.GetDefaultransactionReceiptService(this);
                return _transactionReceiptService;
            }
            set
            {
                _transactionReceiptService = value;
            }
        }

        public Task<TransactionReceipt> SendTransactionAndWaitForReceiptAsync(TransactionInput transactionInput, CancellationTokenSource tokenSource)
        {
            return TransactionReceiptService.SendRequestAndWaitForReceiptAsync(transactionInput, tokenSource);
        }
               
        public virtual Task<HexBigInteger> EstimateGasAsync(CallInput callInput)
        {
            if (Client == null) throw new NullReferenceException("Client not configured");
            if (callInput == null) throw new ArgumentNullException(nameof(callInput));
            var ethEstimateGas = new EthEstimateGas(Client);
            return ethEstimateGas.SendRequestAsync(callInput);
        }

        public abstract Task<string> SendTransactionAsync(TransactionInput transactionInput);
        
        public virtual Task<string> SendTransactionAsync(string from, string to, HexBigInteger amount)
        {  
            return SendTransactionAsync(new TransactionInput() { From = from, To = to, Value = amount});
        }

        public Task<Fee1559> CalculateFee1559Async(BigInteger? maxPriorityFeePerGas = null)
        {
            if (maxPriorityFeePerGas == null) maxPriorityFeePerGas = DefaultMaxPriorityFeePerGas;
            if (Client == null) throw new NullReferenceException("Client not configured");
           
            return Fee1559SuggestionStrategy.SuggestFeeAsync(maxPriorityFeePerGas);
        }

        public async Task<HexBigInteger> GetGasPriceAsync(TransactionInput transactionInput)
        {
            if (transactionInput.GasPrice != null) return transactionInput.GasPrice;
            if (DefaultGasPrice >= 0) return new HexBigInteger(DefaultGasPrice);
            var ethGetGasPrice = new EthGasPrice(Client);
            return await ethGetGasPrice.SendRequestAsync().ConfigureAwait(false);
        }

        protected void SetDefaultGasPriceAndCostIfNotSet(TransactionInput transactionInput)
        {
            if (DefaultGasPrice != -1)
            {
                if (transactionInput.GasPrice == null) transactionInput.GasPrice = new HexBigInteger(DefaultGasPrice);
            }

            if (DefaultGas != null)
            {
                if (transactionInput.Gas == null) transactionInput.Gas = new HexBigInteger(DefaultGas);
            }
        }

        protected void SetDefaultGasIfNotSet(TransactionInput transactionInput)
        {
            if (DefaultGas != null)
            {
                if (transactionInput.Gas == null) transactionInput.Gas = new HexBigInteger(DefaultGas);
            }
        }
#endif
    }
}