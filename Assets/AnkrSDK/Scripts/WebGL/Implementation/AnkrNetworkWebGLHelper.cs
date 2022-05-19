using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.Utils
{
	public class AnkrNetworkMobileHelper : INetworkHelper
	{
		private static readonly string _networkQueryParameter = "network";
		private static readonly string _logoQueryParameter = "logo";
		private static readonly string _deepLinkURL = "https://metamask.app.link/dapp";
		private static readonly string _switchNetworkPageLink = "switch-page.com";

		public Task AddAndSwitchNetwork(EthereumNetwork network, string logoIconURL = null)
		{
			var url = GetURLFromNetwork(network, logoIconURL);
			AddAndSwitchCustomNetwork(url);
			
			return Task.CompletedTask;
		}

		private static void AddAndSwitchCustomNetwork(string url)
		{
			Debug.Log(url);
//			Application.OpenURL(url);
		}

		private static string GetURLFromNetwork(EthereumNetwork network, string logoIcon = null)
		{
			var queryParams = new Dictionary<string, string[]>();
			queryParams.Add(_networkQueryParameter, new []{JsonConvert.SerializeObject(network)});
			if (logoIcon != null)
			{
				queryParams.Add(_logoQueryParameter, new []{logoIcon});
			}

			var query = AnkrSDKHelper.ToQueryString(queryParams);

			return $"{_deepLinkURL}/{_switchNetworkPageLink}?{query}";
		}
	}
}