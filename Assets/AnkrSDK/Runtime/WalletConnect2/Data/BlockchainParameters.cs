using System;
using WalletConnectSharp.Sign.Models;

namespace AnkrSDK.WalletConnect2
{
	// ChainNamespace and Chains array format is defined by
	// Chain Agnostic Improvement Proposals
	// https://github.com/ChainAgnostic/CAIPs
	[Serializable]
	public class BlockchainParameters
	{
		public bool Enabled;
		public string ChainNamespace;
		public string[] Methods;
		public string[] ChainIds;
		public string[] Events;

		public RequiredNamespace ToRequiredNamespace()
		{
			return new RequiredNamespace
			{
				Chains = ChainIds, Methods = Methods, Events = Events
			};
		}
	}
}