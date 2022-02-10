using System;
using System.Linq;
using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Models
{
	[Serializable]
	public class ClientMeta
	{
		[JsonProperty("description")] public string Description;

		[JsonProperty("url")] public string URL;

		[JsonProperty("icons")] public string[] Icons;

		[JsonProperty("name")] public string Name;

		public override bool Equals(object obj)
		{
			if (obj is ClientMeta clientMeta)
			{
				return Description == clientMeta.Description
				       && URL == clientMeta.URL
				       && Icons.SequenceEqual(clientMeta.Icons)
				       && Name == clientMeta.Name;
			}

			return base.Equals(obj);
		}
	}
}