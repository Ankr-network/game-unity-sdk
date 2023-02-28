using System;
using WalletConnectSharp.Sign.Models;

namespace AnkrSDK.WalletConnect2
{
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
				Chains = Chains,
				Methods = Methods,
				Events = Events
			};
		}
	}
}