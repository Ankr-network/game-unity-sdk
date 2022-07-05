using UnityEngine;

namespace AnkrSDK.UseCases.WebGlLogin
{
	[CreateAssetMenu(fileName = "New Wallet", menuName = "Wallet")]
	public class WalletItem : ScriptableObject
	{
		public WebGL.SupportedWallets Type;
		public string Name;
		public Sprite Logo;
	}
}