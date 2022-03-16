﻿using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts.MessageEncodingServices;
using Nethereum.RPC.TransactionManagers;

namespace Nethereum.Contracts.DeploymentHandlers
{
#if !DOTNET35
    public abstract class DeploymentHandlerBase<TContractDeploymentMessage> : ContractTransactionHandlerBase
        where TContractDeploymentMessage : ContractDeploymentMessage, new()
    {
        protected DeploymentMessageEncodingService<TContractDeploymentMessage> DeploymentMessageEncodingService { get; set;}

        private void InitialiseEncodingService()
        {
            DeploymentMessageEncodingService = new DeploymentMessageEncodingService<TContractDeploymentMessage>();
            DeploymentMessageEncodingService.DefaultAddressFrom = GetAccountAddressFrom();
        }

        protected DeploymentHandlerBase(ITransactionManager transactionManager) : base(transactionManager)
        {
            InitialiseEncodingService();
        }
    }
#endif
}