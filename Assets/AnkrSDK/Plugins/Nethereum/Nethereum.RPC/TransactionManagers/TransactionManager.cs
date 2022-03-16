﻿using System;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;

namespace Nethereum.RPC.TransactionManagers
{
    public class TransactionManager : TransactionManagerBase
    {
        public override BigInteger DefaultGas { get; set; }

        public TransactionManager(IClient client)
        {
            this.Client = client;
        }

#if !DOTNET35
        
        public override Task<string> SignTransactionAsync(TransactionInput transaction)
        {
            throw new InvalidOperationException("Default transaction manager cannot sign offline transactions");
        }

        public override Task<string> SendTransactionAsync(TransactionInput transactionInput)
        {
            if (Client == null) throw new NullReferenceException("Client not configured");
            if (transactionInput == null) throw new ArgumentNullException(nameof(transactionInput));
            
            if(IsTransactionToBeSendAsEIP1559(transactionInput)) 
            {
                SetDefaultGasIfNotSet(transactionInput);
            }
            else
            {
                SetDefaultGasPriceAndCostIfNotSet(transactionInput);
            }
            
            return new EthSendTransaction(Client).SendRequestAsync(transactionInput);
        }
#endif
    }

}