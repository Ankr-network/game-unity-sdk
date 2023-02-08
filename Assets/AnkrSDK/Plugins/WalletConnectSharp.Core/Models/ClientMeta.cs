using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace AnkrSDK.WalletConnectSharp.Core.Models
{
	[Serializable]
	public class ClientMeta
	{
		[FormerlySerializedAs("_description")]
		[JsonProperty("description")] public string Description = "Wallet";

		[FormerlySerializedAs("_url")]
		[JsonProperty("url")] public string Url = "https://www.ankr.com/";

		[FormerlySerializedAs("_icons")]
		[JsonProperty("icons")] public string[] Icons = {
			"https://www.ankr.com/static/favicon/apple-touch-icon.png"
		};

		[FormerlySerializedAs("_name")]
		[JsonProperty("name")] public string Name = "Wallet";

		public override bool Equals(object obj)
		{
			if (obj is ClientMeta clientMeta)
			{
				return Description == clientMeta.Description
				       && Url == clientMeta.Url
				       && Icons.SequenceEqual(clientMeta.Icons)
				       && Name == clientMeta.Name;
			}

			return base.Equals(obj);
		}
	}
}