﻿using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Services;
using Nethereum.RPC.TransactionManagers;
using System;
using Nethereum.RPC.Eth.Transactions;

namespace Nethereum.RPC
{
    public class EthApiService : RpcClientWrapper, IEthApiService
    {
        private BlockParameter _defaultBlock;
        private ITransactionManager _transactionManager;

        public EthApiService(IClient client) : this(client, 
            new TransactionManager(client))
        {
        }

        public EthApiService(IClient client, ITransactionManager transactionManager) : base(client)
        {
            Client = client;
            
            ChainId = new EthChainId(client);
            Accounts = new EthAccounts(client);
            CoinBase = new EthCoinBase(client);
            GasPrice = new EthGasPrice(client);
            GetBalance = new EthGetBalance(client);
            GetCode = new EthGetCode(client);
            GetStorageAt = new EthGetStorageAt(client);
            ProtocolVersion = new EthProtocolVersion(client);
            Sign = new EthSign(client);
            Syncing = new EthSyncing(client);

            Transactions = new EthApiTransactionsService(client);
            Filters = new EthApiFilterService(client);
            Blocks = new EthApiBlockService(client);
            Uncles = new EthApiUncleService(client);
            Mining = new EthApiMiningService(client);
            Compile = new EthApiCompilerService(client);
            FeeHistory = new EthFeeHistory(Client);

            DefaultBlock = BlockParameter.CreateLatest();
            TransactionManager = transactionManager;
            TransactionManager.Client = client; //Ensure is the same
        }

        public BlockParameter DefaultBlock
        {
            get { return _defaultBlock; }
            set
            {
                _defaultBlock = value;
                SetDefaultBlock();
            }
        }

        /// <summary>
        /// Returns the currently configured chain id, a value used in replay-protected transaction signing as introduced by [EIP-155](https://github.com/ethereum/EIPs/blob/master/EIPS/eip-155.md).
        /// </summary>
        public IEthChainId ChainId { get; private set; }

        public IEthAccounts Accounts { get; private set; }

        /// <summary>
        /// The address owned by the client that is used as default for things like the mining reward"
        /// </summary>
        public IEthCoinBase CoinBase { get; private set; }

        /// <summary>
        /// Returns the current price per gas in wei
        /// </summary>
        public IEthGasPrice GasPrice { get; private set; }

        /// <summary>
        /// Returns Ether balance of a given or account or contract
        /// </summary>
        public IEthGetBalance GetBalance { get; }

        /// <summary>
        /// Returns code at a given contract address
        /// </summary>
        public IEthGetCode GetCode { get; }

        /// <summary>
        /// Gets a storage value from a contract address, a position, and an optional blockNumber
        /// </summary>
        public IEthGetStorageAt GetStorageAt { get; }

        /// <summary>
        /// The current ethereum protocol version
        /// </summary>
        public IEthProtocolVersion ProtocolVersion { get; private set; }

        /// <summary>
        ///     Signs data with a given address.
        ///     Note the address to sign must be unlocked.
        /// </summary>
        public IEthSign Sign { get; private set; }

        public IEthSyncing Syncing { get; private set; }

        public IEthApiTransactionsService Transactions { get; }

        public IEthApiUncleService Uncles { get; private set; }
        public IEthApiMiningService Mining { get; private set; }
        public IEthApiBlockService Blocks { get; private set; }

        public IEthApiFilterService Filters { get; private set; }

        public IEthFeeHistory FeeHistory { get; private set; }

        public IEthApiCompilerService Compile { get; private set; }
#if !DOTNET35
        public virtual IEtherTransferService  GetEtherTransferService()
        {
            return new EtherTransferService(TransactionManager);
        }
#endif
        public virtual ITransactionManager TransactionManager
        {
            get { return _transactionManager; }
            set { _transactionManager = value; }
        }

        private void SetDefaultBlock()
        {
            GetBalance.DefaultBlock = DefaultBlock;
            GetCode.DefaultBlock = DefaultBlock;
            GetStorageAt.DefaultBlock = DefaultBlock;
            Transactions.SetDefaultBlock(_defaultBlock);
        }
    }
}
