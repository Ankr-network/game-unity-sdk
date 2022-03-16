﻿using Common.Logging;
using Nethereum.BlockchainProcessing.BlockProcessing;
using Nethereum.BlockchainProcessing.Orchestrator;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using Nethereum.RPC.Eth.Blocks;

namespace Nethereum.BlockchainProcessing
{
	public class BlockchainCrawlingProcessor : BlockchainProcessor
	{
		public BlockCrawlOrchestrator Orchestrator => (BlockCrawlOrchestrator)BlockchainProcessingOrchestrator;

		public BlockchainCrawlingProcessor(IBlockchainProcessingOrchestrator blockchainProcessingOrchestrator,
			IBlockProgressRepository blockProgressRepository,
			ILastConfirmedBlockNumberService lastConfirmedBlockNumberService, ILog log = null) : base(
			blockchainProcessingOrchestrator, blockProgressRepository, lastConfirmedBlockNumberService, log)
		{
		}
	}
}