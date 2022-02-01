using System;
using System.Threading.Tasks;
using MirageSDK.Core.Infrastructure;
using Nethereum.JsonRpc.Client;
using Nethereum.Signer;
using Nethereum.Web3;
using WalletConnectSharp.NEthereum;
using WalletConnectSharp.Unity;

namespace MirageSDK.Core.Implementation
{
	public class MirageSDKWrapper : IMirageSDK
	{
		private readonly IWeb3 _provider;
		private readonly IClient _client;

		private MirageSDKWrapper(string providerURL)
		{
			var wcProtocol = WalletConnect.Instance.Session;
			_client = wcProtocol.CreateProvider(new Uri(providerURL));
			_provider = new Web3(_client);
		}

		public static IMirageSDK GetInitializedInstance(string providerURL)
		{
			return new MirageSDKWrapper(providerURL);
		}

		public IContract GetContract(string address, string abi)
		{
			return new Contract(_provider, _client, address, abi);
		}

		public Task<string> Sign(string message)
		{
			return WalletConnect.ActiveSession.EthSign(WalletConnect.ActiveSession.Accounts[0], message);
		}

		public string CheckSignature(string message, string signature)
		{
			var signer = new EthereumMessageSigner();
			return signer.EncodeUTF8AndEcRecover(message, signature);
		}
	}
}