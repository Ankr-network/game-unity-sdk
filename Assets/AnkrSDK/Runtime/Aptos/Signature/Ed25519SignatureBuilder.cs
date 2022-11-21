using System.Linq;
using System.Text;
using AnkrSDK.Aptos.DTO;
using Mirage.Aptos.Imlementation.ABI;
using Mirage.Aptos.Utils;
using Org.BouncyCastle.Crypto.Digests;

namespace AnkrSDK.Aptos
{
	public class Ed25519SignatureBuilder
	{
		private const string SignatureSalt = "APTOS::RawTransaction";
		private const string TransactionType = "ed25519_signature";

		public Ed25519Signature GetSignature(Account sender, RawTransaction rawTransaction)
		{
			var signingMessage = DataUtils.Serialize(rawTransaction);

			return new Ed25519Signature
			{
				Type = TransactionType,
				PublicKey = sender.PublicKey,
				Signature = Sign(sender, signingMessage)
			};
		}
		
		private string Sign(Account sender, byte[] transaction)
		{
			var salt = GetSalt();
			var signingMessage = salt.Concat(transaction).ToArray();

			var signature = sender.Sign(signingMessage);
			return signature.Take(64).ToArray().ToHexCompact(true);
		}

		private byte[] GetSalt()
		{
			var digest = new Sha3Digest();
			var salt = Encoding.ASCII.GetBytes(SignatureSalt);
			var result = new byte[digest.GetDigestSize()];
			digest.BlockUpdate(salt, 0, salt.Length);
			digest.DoFinal(result, 0);
			return result;
		}
	}
}