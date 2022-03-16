﻿using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Fee1559Suggestions;
using Nethereum.Util;

namespace Nethereum.RPC.TransactionManagers
{
#if !DOTNET35
    public class EtherTransferService : IEtherTransferService
    {
        private readonly ITransactionManager _transactionManager;

        public EtherTransferService(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager ?? throw new ArgumentNullException(nameof(transactionManager));
        }

        public Task<string> TransferEtherAsync(string toAddress, decimal etherAmount, decimal? gasPriceGwei = null, BigInteger? gas = null, BigInteger? nonce = null)
        {
            var fromAddress = _transactionManager?.Account?.Address;
            var transactionInput = EtherTransferTransactionInputBuilder.CreateTransactionInput(fromAddress, toAddress, etherAmount, gasPriceGwei, gas, nonce);
            return _transactionManager.SendTransactionAsync(transactionInput);
        }

        public Task<TransactionReceipt> TransferEtherAndWaitForReceiptAsync(string toAddress, decimal etherAmount, decimal? gasPriceGwei = null, BigInteger? gas = null, BigInteger? nonce = null, CancellationTokenSource tokenSource = null)
        {
            var fromAddress = _transactionManager?.Account?.Address;
            var transactionInput = EtherTransferTransactionInputBuilder.CreateTransactionInput(fromAddress, toAddress, etherAmount, gasPriceGwei, gas, nonce);
            return _transactionManager.SendTransactionAndWaitForReceiptAsync(transactionInput, tokenSource);
        }

        public async Task<decimal> CalculateTotalAmountToTransferWholeBalanceInEtherAsync(string address, decimal gasPriceGwei, BigInteger? gas = null)
        {
            var ethGetBalance = new EthGetBalance(_transactionManager.Client);
            var currentBalance = await ethGetBalance.SendRequestAsync(address).ConfigureAwait(false);
            var gasPrice = UnitConversion.Convert.ToWei(gasPriceGwei, UnitConversion.EthUnit.Gwei);
            var gasAmount = gas ?? _transactionManager.DefaultGas;

            var totalAmount = currentBalance.Value - (gasAmount * gasPrice);
            if (totalAmount <= 0) throw new Exception("Insufficient balance to make a transfer");
            return UnitConversion.Convert.FromWei(totalAmount);
        }

    

        public Task<TransactionReceipt> TransferEtherAndWaitForReceiptAsync(string toAddress, decimal etherAmount, BigInteger maxPriorityFeePerGas, BigInteger maxFeePerGas, BigInteger? gas = null, BigInteger? nonce = null, CancellationTokenSource tokenSource = null)
        {
            var fromAddress = _transactionManager?.Account?.Address;
            var transactionInput = EtherTransferTransactionInputBuilder.CreateTransactionInput(fromAddress, toAddress, etherAmount, maxPriorityFeePerGas, maxFeePerGas, gas, nonce);
            return _transactionManager.SendTransactionAndWaitForReceiptAsync(transactionInput, tokenSource);
        }

        public Task<string> TransferEtherAsync(string toAddress, decimal etherAmount, BigInteger maxPriorityFeePerGas, BigInteger maxFeePerGas, BigInteger? gas = null, BigInteger? nonce = null)
        {
            //Make the the maxPriorityFee and maxFeePerGas
            var fromAddress = _transactionManager?.Account?.Address;
            var transactionInput = EtherTransferTransactionInputBuilder.CreateTransactionInput(fromAddress, toAddress, etherAmount, maxPriorityFeePerGas, maxFeePerGas, gas, nonce);
            return _transactionManager.SendTransactionAsync(transactionInput);
        }

        public async Task<Fee1559> SuggestFeeToTransferWholeBalanceInEtherAsync(
            BigInteger? maxPriorityFeePerGas = null)
        {
            
            var fee1559 = await _transactionManager.Fee1559SuggestionStrategy.SuggestFeeAsync(maxPriorityFeePerGas).ConfigureAwait(false);
            //Match it so there are not crumbs
            fee1559.MaxPriorityFeePerGas = fee1559.MaxFeePerGas;
            return fee1559;
        }

        public async Task<decimal> CalculateTotalAmountToTransferWholeBalanceInEtherAsync(string address, BigInteger maxFeePerGas, BigInteger? gas = null)
        {
            var ethGetBalance = new EthGetBalance(_transactionManager.Client);
            var currentBalance = await ethGetBalance.SendRequestAsync(address).ConfigureAwait(false);

            var gasAmount = gas ?? _transactionManager.DefaultGas;

            var totalAmount = currentBalance.Value - (gasAmount * maxFeePerGas);
            if (totalAmount <= 0) throw new Exception("Insufficient balance to make a transfer");
            return UnitConversion.Convert.FromWei(totalAmount);
        }

        public async Task<BigInteger> EstimateGasAsync(string toAddress, decimal etherAmount)
        {
            var fromAddress = _transactionManager?.Account?.Address;
            var callInput = (CallInput)EtherTransferTransactionInputBuilder.CreateTransactionInput(fromAddress, toAddress, etherAmount);
            var hexEstimate = await _transactionManager.EstimateGasAsync(callInput).ConfigureAwait(false);
            return hexEstimate.Value;
        }

        
    }
#endif
}