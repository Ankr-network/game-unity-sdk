using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace AnkrSDK.Aptos
{
	public class AptosTransaction : MonoBehaviour
	{
		private readonly byte[] _alicePrivateKey = new byte[] {255,211,113,35,165,87,101,140,224,222,92,33,154,65,150,110,140,93,2,42,28,171,127,97,43,26,129,71,81,123,43,127,184,15,11,253,79,245,134,84,235,194,101,199,183,86,195,6,154,234,47,136,15,71,94,119,91,201,60,202,25,182,116,124};
		private readonly byte[] _bobPrivateKey = new byte[] {20,225,26,255,205,22,88,59,107,19,118,175,43,243,114,90,198,51,246,142,39,197,124,171,28,60,58,214,46,21,146,143,18,48,212,127,244,102,145,168,135,173,213,67,205,70,19,246,184,65,43,96,86,181,231,77,73,199,154,39,255,120,32,129};
		[SerializeField] private TMP_Text _textField;
		private void Start()
		{
			Check().Forget();
		}

		private async UniTask Check()
		{
			var client = new Client("https://fullnode.devnet.aptoslabs.com");
			var coinClient = new CoinClient(client);
			
			AddText("------ Accounts -------");
			var from = new Account(_alicePrivateKey);
			AddText($"Alice: {from.GetAddress()}");
			var to = new Account(_bobPrivateKey);
			AddText($"Bob: {to.GetAddress()}");
			
			AddText("------ Balances --------");
			AddText($"Alice's balance: {await coinClient.GetBalance(from)}");
			AddText($"Bob's balance: {await coinClient.GetBalance(to)}");
			
			AddText("------ Transaction --------");
			var hash = await coinClient.Transfer(from, to, 1000);
			AddText("hash = " + hash);
			Debug.Log("hash = " + hash);
		}

		private void AddText(string newText)
		{
			_textField.text += newText + "\n";
		}
	}
}