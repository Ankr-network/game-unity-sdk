using System.Linq;

namespace MirageSDK.Data
{
	public static class SupportedWallets
	{
		public static Wallet[] WebGL =
		{
			Wallet.None,
			Wallet.Metamask,
			Wallet.Torus
		};
		public static Wallet[] Mobile =
		{
			Wallet.None,
			Wallet.Metamask,
			Wallet.Trust,
			Wallet.Argent,
			Wallet.Rainbow,
			Wallet.Pillar
		};
		public static Wallet[] Standalone =
		{
			Wallet.None,
			Wallet.Metamask,
			Wallet.Trust,
			Wallet.Argent,
			Wallet.Rainbow,
			Wallet.Pillar
		};

		public static string[] GetWebGLWallets()
		{
			return WalletsToArray(WebGL);
		}
		
		public static string[] GetMobileWallets()
		{
			return WalletsToArray(Mobile);
		}
		
		public static string[] GetStandaloneWallets()
		{
			return WalletsToArray(Standalone);
		}
		
		public static string[] WalletsToArray(Wallet[] wallets)
		{
			return wallets.Select(wallet => wallet.ToString()).ToArray();
		}
	}
}