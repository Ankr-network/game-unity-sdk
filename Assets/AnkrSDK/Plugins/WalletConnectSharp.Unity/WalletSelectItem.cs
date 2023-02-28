using System;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	[Serializable]
	public class WalletSelectItem
	{
		public string Id;
		public string Name;
		public bool Selected = false;

		public WalletSelectItem(string id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}