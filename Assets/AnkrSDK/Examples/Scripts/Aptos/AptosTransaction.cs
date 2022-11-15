using UnityEngine;

namespace AnkrSDK.Aptos
{
	public class AptosTransaction : MonoBehaviour
	{
		private void Start()
		{
			Check();
		}

		public void Check()
		{
			var privateKey = new byte[] {255,211,113,35,165,87,101,140,224,222,92,33,154,65,150,110,140,93,2,42,28,171,127,97,43,26,129,71,81,123,43,127,184,15,11,253,79,245,134,84,235,194,101,199,183,86,195,6,154,234,47,136,15,71,94,119,91,201,60,202,25,182,116,124};
			var account = new Account(privateKey);
			Debug.Log(account.GetAddress());
		}
	}
}