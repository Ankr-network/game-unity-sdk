using Chaos.NaCl;
using Mirage.Aptos.Utils;
using Org.BouncyCastle.Crypto.Digests;

namespace AnkrSDK.Aptos
{
	public class Account
	{
		private const byte AddressEnding = 0x00;
		
		private byte[] _publicKey;
		private byte[] _privateKey;
		
		public readonly string Address;
		public readonly string PublicKey;

		public Account()
		{
			var seed = GenerateRandomSeed();
			CreateKeyPairFromSeed(seed);
			Address = GetAddress();
			PublicKey = _publicKey.ToHex(true);
		}

		public Account(byte[] privateKey)
		{
			var seed = privateKey.Slice(0, 32);
			CreateKeyPairFromSeed(seed);
			Address = GetAddress();
			PublicKey = _publicKey.ToHex(true);
		}

		private static byte[] GenerateRandomSeed()
		{
			byte[] bytes = new byte[Ed25519.PrivateKeySeedSizeInBytes];
			RandomUtils.GetBytes(bytes);
			return bytes;
		}

		private void CreateKeyPairFromSeed(byte[] seed)
		{
			_privateKey = Ed25519.ExpandedPrivateKeyFromSeed(seed);
			_publicKey = Ed25519.PublicKeyFromSeed(seed);
		}

		public string GetAddress()
		{
			var digest = new Sha3Digest();
			var result = new byte[digest.GetDigestSize()];
			digest.BlockUpdate(_publicKey, 0, _publicKey.Length);
			digest.Update(AddressEnding);
			digest.DoFinal(result, 0);
			return result.ToHex(true);
		}

		public byte[] Sign(byte[] message)
		{
			return Ed25519.Sign(message, _privateKey);
		}
	}
}