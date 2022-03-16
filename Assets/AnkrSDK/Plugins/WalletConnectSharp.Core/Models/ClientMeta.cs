using System;
using System.Linq;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models
{
	[Serializable]
	public class ClientMeta
	{
		[JsonProperty("description")] public string _description = "Wallet";

		[JsonProperty("url")] public string _url = "https://www.ankr.com/";

		[JsonProperty("icons")] public string[] _icons = {
			"https://www.ankr.com/static/favicon/apple-touch-icon.png"
		};

		[JsonProperty("name")] public string _name = "Wallet";

		public override bool Equals(object obj)
		{
			if (obj is ClientMeta clientMeta)
			{
				return _description == clientMeta._description
				       && _url == clientMeta._url
				       && _icons.SequenceEqual(clientMeta._icons)
				       && _name == clientMeta._name;
			}

			return base.Equals(obj);
		}
	}
}