using MirageSDK.Data;
using UnityEngine;

namespace MirageSDK.Examples.UseCases.WebGlLogin
{
	[CreateAssetMenu(fileName = "New Wallet", menuName = "Wallet")]
	public class WalletItem : ScriptableObject
	{
		public Wallet Type;
		public string Name;
		public Sprite Logo;
	}
}