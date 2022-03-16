﻿using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.RPC.NonceServices;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.Util;

namespace Nethereum.Web3.Accounts
{
    public class ExternalAccountSignerTransactionManager : TransactionManagerBase
    {
        private readonly LegacyTransactionSigner _transactionSigner;

        public ExternalAccountSignerTransactionManager(IClient rpcClient, ExternalAccount account,
            BigInteger? chainId = null)
        {
            ChainId = chainId;
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Client = rpcClient;
            _transactionSigner = new LegacyTransactionSigner();
        }

        public BigInteger? ChainId { get; }


        public override BigInteger DefaultGas { get; set; } = SignedLegacyTransaction.DEFAULT_GAS_LIMIT;


        public override Task<string> SendTransactionAsync(TransactionInput transactionInput)
        {
            if (transactionInput == null) throw new ArgumentNullException(nameof(transactionInput));
            return SignAndSendTransactionAsync(transactionInput);
        }

        public override Task<string> SignTransactionAsync(TransactionInput transaction)
        {
            return SignTransactionRetrievingNextNonceAsync(transaction);
        }

        public async Task<string> SignTransactionExternallyAsync(TransactionInput transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (!transaction.From.IsTheSameAddress(Account.Address))
                throw new Exception("Invalid account used signing");

            SetDefaultGasPriceAndCostIfNotSet(transaction);

            var nonce = transaction.Nonce;
            if (nonce == null)
                throw new ArgumentNullException(nameof(transaction), "Transaction nonce has not been set");

            var gasPrice = transaction.GasPrice;
            var gasLimit = transaction.Gas;

            var value = transaction.Value ?? new HexBigInteger(0);

            string signedTransaction;

            var externalSigner = ((ExternalAccount) Account).ExternalSigner;

            if (externalSigner.Supported1559 && transaction.Type != null &&
                transaction.Type.Value == TransactionType.EIP1559.AsByte())
            {
                var maxPriorityFeePerGas = transaction.MaxPriorityFeePerGas.Value;
                var maxFeePerGas = transaction.MaxFeePerGas.Value;
                if (ChainId == null) throw new ArgumentException("ChainId required for TransactionType 0X02 EIP1559");

                var transaction1559 = new Transaction1559(ChainId.Value, nonce, maxPriorityFeePerGas, maxFeePerGas,
                    gasLimit, transaction.To, value, transaction.Data,
                    transaction.AccessList.ToSignerAccessListItemArray());
                await transaction1559.SignExternallyAsync(externalSigner).ConfigureAwait(false);
                signedTransaction = transaction1559.GetRLPEncoded().ToHex();
            }
            else
            {
                if (ChainId == null)
                    signedTransaction = await _transactionSigner.SignTransactionAsync(externalSigner,
                        transaction.To,
                        value.Value, nonce,
                        gasPrice.Value, gasLimit.Value, transaction.Data).ConfigureAwait(false);
                else
                    signedTransaction = await _transactionSigner.SignTransactionAsync(externalSigner, ChainId.Value,
                        transaction.To,
                        value.Value, nonce,
                        gasPrice.Value, gasLimit.Value, transaction.Data).ConfigureAwait(false);
            }

            return signedTransaction;
        }


        public string SignTransaction(TransactionInput transaction)
        {
            return SignTransactionRetrievingNextNonceAsync(transaction).Result;
        }

        protected async Task<string> SignTransactionRetrievingNextNonceAsync(TransactionInput transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (!transaction.From.IsTheSameAddress(Account.Address))
                throw new Exception("Invalid account used signing");
            var nonce = await GetNonceAsync(transaction).ConfigureAwait(false);
            transaction.Nonce = nonce;

            var externalSigner = ((ExternalAccount) Account).ExternalSigner;
            if (externalSigner.Supported1559)
            {
                await SetTransactionFeesOrPricingAsync(transaction).ConfigureAwait(false);
            }
            else
            {
                var gasPrice = await GetGasPriceAsync(transaction).ConfigureAwait(false);
                transaction.GasPrice = gasPrice;
            }

            return await SignTransactionExternallyAsync(transaction).ConfigureAwait(false);
        }

        public async Task<HexBigInteger> GetNonceAsync(TransactionInput transaction)
        {
            if (Client == null) throw new NullReferenceException("Client not configured");
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            var nonce = transaction.Nonce;
            if (nonce == null)
            {
                if (Account.NonceService == null)
                    Account.NonceService = new InMemoryNonceService(Account.Address, Client);
                Account.NonceService.Client = Client;
                nonce = await Account.NonceService.GetNextNonceAsync().ConfigureAwait(false);
            }

            return nonce;
        }

        private async Task<string> SignAndSendTransactionAsync(TransactionInput transaction)
        {
            if (Client == null) throw new NullReferenceException("Client not configured");
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (!transaction.From.IsTheSameAddress(Account.Address))
                throw new Exception("Invalid account used signing");

            var ethSendTransaction = new EthSendRawTransaction(Client);
            var signedTransaction = await SignTransactionRetrievingNextNonceAsync(transaction).ConfigureAwait(false);
            return await ethSendTransaction.SendRequestAsync(signedTransaction.EnsureHexPrefix()).ConfigureAwait(false);
        }
    }
}