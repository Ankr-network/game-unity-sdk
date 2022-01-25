using System.Numerics;

namespace Web3Unity.Core
{
    public class Utils
    {
        public static string ConvertNumber(string value)
        {
            BigInteger bnValue = BigInteger.Parse(value);
            return "0x" + bnValue.ToString("X");
        }
    }
}