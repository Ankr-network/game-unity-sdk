using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.Mobile
{
	public class AnkrNetworkHelper : INetworkHelper
	{
		private const string NetworkQueryParameter = "network";
		private const string LogoQueryParameter = "logo";
		private const string DeepLinkURL = "https://metamask.app.link/dapp";
		private const string SwitchNetworkPageLink = "changenetwork-service.game.ankr.com";

		public Task AddAndSwitchNetwork(EthereumNetwork network)
		{
			var url = GetURLFromNetwork(network);
			OpenCustomNetworkSwitchURL(url);
			return Task.CompletedTask;
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
	}
}