using System;
using WalletConnectSharp.Sign.Models;

namespace AnkrSDK.WalletConnect2
{
	// BlockchainId and Chains array format is defined by
	// Chain Agnostic Improvement Proposals
	// https://github.com/ChainAgnostic/CAIPs
	[Serializable]
	public class BlockchainParameters
	{
		public bool Enabled;
		public string BlockchainId;
		public string[] Methods;
		public string[] Chains;
		public string[] Events;

		public RequiredNamespace ToRequiredNamespace()
		{
			return new RequiredNamespace
			{
				Chains = Chains, Methods = Methods, Events = Events
			};
		}
	}
}