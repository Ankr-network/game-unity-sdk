using System.Linq;
using System.Text;
using AnkrSDK.Aptos.Constants;
using AnkrSDK.Aptos.DTO;
using AnkrSDK.Aptos.Imlementation.ABI;
using AnkrSDK.Aptos.Utils;
using Org.BouncyCastle.Crypto.Digests;
using UnityEngine;
using Chaos.NaCl;

namespace AnkrSDK.Aptos
{
	public class AptosTransaction : MonoBehaviour
	{
		private const string RawTransactionSalt = "APTOS::RawTransaction";
		
		private void Start()
		{
			Check();
		}

		public void Check()
		{
			var privateKey = new byte[] {255,211,113,35,165,87,101,140,224,222,92,33,154,65,150,110,140,93,2,42,28,171,127,97,43,26,129,71,81,123,43,127,184,15,11,253,79,245,134,84,235,194,101,199,183,86,195,6,154,234,47,136,15,71,94,119,91,201,60,202,25,182,116,124};
			
			var sender = "0xa8583bfca93e862653cac142fd09ff848249180906036978a8ca8e6a8ee55778";
			var sequenceNumber = 0;
			var maxGasAmount = 200000;
			var gasUnitPrice = 100;
			var expireTimestamp = 1667905991;
			var chainId = 36;
			var payload = CreatePayload();

			var txRaw = new RawTransaction(
				sender.HexToByteArray(),
				(ulong)sequenceNumber,
				payload,
				(ulong)maxGasAmount,
				(ulong)gasUnitPrice,
				(ulong)expireTimestamp,
				(uint)chainId
			);

			var serializer = new Serializer();
			txRaw.Serialize(serializer);
			var message = serializer.GetBytes();

			var signature = SignMessage(message, privateKey);
			
			Debug.Log(signature);
		}
		
		private TransactionPayload CreatePayload()
		{
			var builder = new TransactionBuilderABI(ABIs.GetCoinABIs());

			var func = "0x1::coin::transfer";
			var typeArgs = new string[]
			{
				"0x1::aptos_coin::AptosCoin"
			};
			var args = new object[]
			{
				"0x65d922ec609ecb1b694ffa502938dd4dff4380de90658a5cee84b67a7e78bcbb",
				1000
			};
			
			return builder.BuildTransactionPayload(func, typeArgs, args);
		}

		public string SignMessage(byte[] transaction, byte[] privateKey)
		{
			var salt = GetSalt();
			var signingMessage = salt.Concat(transaction).ToArray();

			var signature = Ed25519.Sign(signingMessage, privateKey);
			return signature.Take(64).ToArray().ToHexCompact(true);
		}

		private byte[] GetSalt()
		{
			var digest = new Sha3Digest();
			var salt = Encoding.ASCII.GetBytes(RawTransactionSalt);
			var result = new byte[digest.GetDigestSize()];
			digest.BlockUpdate(salt, 0, salt.Length);
			digest.DoFinal(result, 0);
			return result;
		}
	}
}