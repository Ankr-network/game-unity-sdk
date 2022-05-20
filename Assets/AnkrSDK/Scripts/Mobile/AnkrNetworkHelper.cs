using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.Utils
{
	public class AnkrNetworkHelper : INetworkHelper
	{
		private static readonly string _networkQueryParameter = "network";
		private static readonly string _logoQueryParameter = "logo";
		private static readonly string _deepLinkURL = "https://metamask.app.link/dapp";
		private static readonly string _switchNetworkPageLink = "game.ankr.com/changenetwork-service";

		public Task AddAndSwitchNetwork(EthereumNetwork network)
		{
			var url = GetURLFromNetwork(network);
			AddAndSwitchCustomNetwork(url);
			
			return Task.CompletedTask;
		}

		private static void AddAndSwitchCustomNetwork(string url)
		{
			Debug.Log(url);
			Application.OpenURL(url);
		}

		private static string GetURLFromNetwork(EthereumNetwork network, string logoIcon = null)
		{
			var uriQueryBuilder = new UriQueryBuilder($"{_deepLinkURL}/{_switchNetworkPageLink}");
			uriQueryBuilder.AppendArgument(_networkQueryParameter, JsonConvert.SerializeObject(network));
			if (logoIcon != null)
			{
				uriQueryBuilder.AppendArgument(_logoQueryParameter, logoIcon);
			}

			return uriQueryBuilder.Build();
		}
	}
}