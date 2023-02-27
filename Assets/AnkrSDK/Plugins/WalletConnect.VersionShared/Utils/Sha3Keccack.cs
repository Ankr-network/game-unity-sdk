using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace AnkrSDK.Plugins.WalletConnect.VersionShared.Utils
{
    public class Sha3Keccack
    {
        public static Sha3Keccack Current { get; } = new Sha3Keccack();

        public string CalculateHash(string value)
        {
            var input = Encoding.UTF8.GetBytes(value);
            var output = CalculateHash(input);
            return global::WalletConnectSharp.Common.Utils.HexByteConvertorExtensions.ToHex(output);
        }

        public string CalculateHashFromHex(params string[] hexValues)
        {
            var joinedHex = string.Join("", hexValues.Select(x => global::WalletConnectSharp.Common.Utils.HexByteConvertorExtensions.RemoveHexPrefix(x)).ToArray());
            return global::WalletConnectSharp.Common.Utils.HexByteConvertorExtensions.ToHex(CalculateHash(global::WalletConnectSharp.Common.Utils.HexByteConvertorExtensions.HexToByteArray(joinedHex)));
        }

        public byte[] CalculateHash(byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }
    }
}