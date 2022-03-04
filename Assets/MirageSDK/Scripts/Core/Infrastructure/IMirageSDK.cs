using System;

namespace MirageSDK.Core.Infrastructure
{
	public enum NetworkName{Ethereum,EthereumRinkebyTestNet,BinanceSmartChain,BinanceSmartChainTestNet}
	public interface IMirageSDK : IContractProvider, ISignatureProvider
	{
		void AddAndSwitchNetwork(NetworkName networkEnum);
		void AddAndSwitchCustomNetwork(string url);
	}
}