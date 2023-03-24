using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.Utils;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;
using Newtonsoft.Json;
using UnityEngine;

namespace MirageSDK.Mobile
{
	public class MirageNetworkHelper : INetworkHelper
	{
		private const string NetworkQueryParameter = "network";
		private const string LogoQueryParameter = "logo";
		private const string DeepLinkURL = "https://metamask.app.link/dapp";
		private const string SwitchNetworkPageLink = "changenetwork-service.game.ankr.com";

		public UniTask AddAndSwitchNetwork(EthereumNetwork network)
		{
			var url = GetURLFromNetwork(network);
			OpenCustomNetworkSwitchURL(url);
			return UniTask.CompletedTask;
		}

		private static void OpenCustomNetworkSwitchURL(string url)
		{
			Application.OpenURL(url);
		}

		private static string GetURLFromNetwork(EthereumNetwork network, string logoIcon = null)
		{
			var uriQueryBuilder = new UriQueryBuilder($"{DeepLinkURL}/{SwitchNetworkPageLink}");
			uriQueryBuilder.AppendArgument(NetworkQueryParameter, JsonConvert.SerializeObject(network));
			if (logoIcon != null)
			{
				uriQueryBuilder.AppendArgument(LogoQueryParameter, logoIcon);
			}

			return uriQueryBuilder.Build();
		}

		public UniTask AddAndSwitchNetwork(EthChainData chain)
		{
			throw new System.NotImplementedException();
		}
	}
}