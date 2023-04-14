using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace MirageSDK.WalletConnectSharp.Core.Models
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

		public override int GetHashCode()
		{
			const int prime1 = 17;
			const int prime2 = 31;
			
			int hash = prime1;
			hash = hash * prime2 + Description?.GetHashCode() ?? 0;
			hash = hash * prime2 + Url?.GetHashCode() ?? 0;
			hash = hash * prime2 + Name?.GetHashCode() ?? 0;

			if (Icons != null)
			{
				foreach (var icon in Icons)
				{
					hash = hash * prime2 + icon?.GetHashCode() ?? 0;
				}
			}

			return hash;
		}
	}
}