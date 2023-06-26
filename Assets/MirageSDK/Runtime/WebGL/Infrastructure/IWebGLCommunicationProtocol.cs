﻿using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Data;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;
using MirageSDK.WebGL.DTO;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace MirageSDK.WebGL.Infrastructure
{
	internal interface IWebGLCommunicationProtocol
	{
		void Init();
		UniTask ConnectTo(Wallet wallet, EthereumNetwork chain);
		void Disconnect();
		UniTask<WalletsStatus> GetWalletsStatus();
		UniTask<string> SendTransaction(TransactionData transaction);
		UniTask<string> GetContractData(TransactionData transaction);
		UniTask<HexBigInteger> EstimateGas(TransactionData transaction);
		UniTask<string> Sign(DataSignaturePropsDTO signProps);
		UniTask<string> GetDefaultAccount();
		UniTask<BigInteger> GetChainId();
		UniTask<Transaction> GetTransaction(string transactionHash);
		UniTask AddChain(EthChainData networkData);
		UniTask UpdateChain(EthUpdateChainData networkData);
		UniTask SwitchChain(EthChain networkData);
		UniTask<BigInteger> GetBalance();
		UniTask<BigInteger> GetBlockNumber();
		UniTask<BigInteger> GetBlockTransactionCount(string blockId);
		UniTask<TResultType> GetBlock<TResultType>(string blockId, bool returnTransactionObjects);
		UniTask<FilterLog[]> GetEvents(NewFilterInput filters);
		UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash);
	}
}