using Cysharp.Threading.Tasks;
using Mirage.Aptos.SDK;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace AnkrSDK.Aptos
{
	public class AptosTransaction : MonoBehaviour
	{
		private readonly byte[] _alicePrivateKey = new byte[] {255,211,113,35,165,87,101,140,224,222,92,33,154,65,150,110,140,93,2,42,28,171,127,97,43,26,129,71,81,123,43,127,184,15,11,253,79,245,134,84,235,194,101,199,183,86,195,6,154,234,47,136,15,71,94,119,91,201,60,202,25,182,116,124};
		private readonly byte[] _bobPrivateKey = new byte[] {20,225,26,255,205,22,88,59,107,19,118,175,43,243,114,90,198,51,246,142,39,197,124,171,28,60,58,214,46,21,146,143,18,48,212,127,244,102,145,168,135,173,213,67,205,70,19,246,184,65,43,96,86,181,231,77,73,199,154,39,255,120,32,129};
		// private readonly byte[] _alicePrivateKey = "0xb50675417de5434afdaf0de153cd687e9373576294c4e180ab441096f819aa03".HexToByteArray();
		// private readonly byte[] _bobPrivateKey = "0xf15a7e24066228b7bdb6de001961eb2cf7cbb995a54cf21cead95f0b5b2f3985".HexToByteArray();
		[SerializeField] private TMP_Text _textField;

		private Client _client;
		private CoinClient _coinClient;
		private TokenClient _tokenClient;

		private void Start()
		{
			_client = new Client("https://fullnode.devnet.aptoslabs.com");
			_coinClient = new CoinClient(_client);
			_tokenClient = new TokenClient(_client);
			Check().Forget();
		}

		private async UniTask Check()
		{
			await CreateCollection();
		}
		
		private async UniTask TransferCoins()
		{
			AddText("------ Accounts -------");
			var from = new Account(_alicePrivateKey);
			AddText($"Alice: {from.GetAddress()}");
			var to = new Account(_bobPrivateKey);
			AddText($"Bob: {to.GetAddress()}");
			
			AddText("------ Balances --------");
			AddText($"Alice's balance: {await _coinClient.GetBalance(from)}");
			AddText($"Bob's balance: {await _coinClient.GetBalance(to)}");
			
			AddText("------ Transaction --------");
			var hash = await _coinClient.Transfer(from, to, 1000);
			AddText("hash = " + hash);
			Debug.Log("hash = " + hash);

			var awaiter = new TransactionAwaiter(_client);
			var transaction = await awaiter.WaitForTransactionWithResult(hash);
			
			Debug.Log(JsonConvert.SerializeObject(transaction));
		}

		private async UniTask RequestTransaction(string hash)
		{
			var tx = await _client.GetTransactionByHash(hash);
			// Debug.Log(JsonConvert.SerializeObject(tx));
		}
		
		private async UniTask RequestMinedTransaction(string hash)
		{
			var awaiter = new TransactionAwaiter(_client);
			var transaction = await awaiter.WaitForTransactionWithResult(hash);
			Debug.Log(JsonConvert.SerializeObject(transaction));
		}

		private async UniTask CreateCollection()
		{
			var from = new Account(_alicePrivateKey);
			var to = new Account(_bobPrivateKey);
			
			AddText("----- Create collection -----");
			var collectionName = "Mirage Aptos SDK 4";
			var description = "Collection for test Aptos SDK";
			var uri = "https://mirage.xyz/";

			var hash = await _tokenClient.CreateCollection(from, collectionName, description, uri);
			AddText(hash.Hash);
			
			await RequestTransaction(hash.Hash);

			AddText("----- Create token -----");
			var tokenName = "Mirages's first token";
			var tokenDescription = "Mirages's simple token";
			var hash1 = await _tokenClient.CreateToken(
				from,
				collectionName,
				tokenName,
				description,
				1,
				"https://mirage.xyz/_next/static/videos/video-desktop-8511e2ee740953e08e74b95f401399f7.webm"
				);
			
			AddText(hash1.Hash);
			await RequestTransaction(hash1.Hash);

			AddText("----- Offer token -----");
			var tokenPropertyVersion = 0;
			var hash3 = await _tokenClient.OfferToken(
				from,
				to.Address,
				from.Address,
				collectionName,
				tokenName,
				1,
				tokenPropertyVersion
				);
			
			AddText(hash3.Hash);
			await RequestTransaction(hash3.Hash);

			AddText("----- Claim token -----");
			var hash4 = await _tokenClient.ClaimToken(
				to,
				from.Address,
				from.Address,
				collectionName,
				tokenName,
				tokenPropertyVersion
			);
			
			AddText(hash4.Hash);
			await RequestTransaction(hash4.Hash);
		}

		private void AddText(string newText)
		{
			Debug.Log(newText);
			_textField.text += newText + "\n";
		}
	}
}