using Org.BouncyCastle.Security;
using System;
using System.Security.Cryptography;

namespace AnkrSDK.Aptos
{
    /// <summary>
    /// Implements utilities to be used with random number generation.
    /// </summary>
    public static class RandomUtils
    {
        /// <summary>
        /// Whether to use additional entropy or not.
        /// </summary>
        public static bool UseAdditionalEntropy { get; set; } = true;

        /// <summary>
        /// Initialize the static instance of the random number generator.
        /// </summary>
        static RandomUtils()
        {
            Random = new SecureRandom();
            AddEntropy(Guid.NewGuid().ToByteArray());
        }

        /// <summary>
        /// The random number generator.
        /// </summary>
        public static SecureRandom Random
        {
            get;
            set;
        }

        /// <summary>
        /// Get random bytes.
        /// </summary>
        /// <param name="length">The number of bytes to get.</param>
        /// <returns>The byte array.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the random number generator has not been initialized</exception>
        public static byte[] GetBytes(int length)
        {
            byte[] data = new byte[length];
            if (Random == null)
                throw new InvalidOperationException("You must initialize the random number generator before generating numbers.");
            Random.NextBytes(data);
            PushEntropy(data);
            return data;
        }

        /// <summary>
        /// Get random bytes.
        /// </summary>
        /// <param name="output">The array of bytes to write the random bytes to.</param>
        /// <returns>The byte array.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the random number generator has not been initialized</exception>
        public static byte[] GetBytes(byte[] output)
        {
            if (Random == null)
                throw new InvalidOperationException("You must initialize the random number generator before generating numbers.");
            Random.NextBytes(output);
            PushEntropy(output);
            return output;
        }

        /// <summary>
        /// Pushes entropy to the given array of bytes.
        /// </summary>
        /// <param name="data">The array of bytes.</param>
        private static void PushEntropy(byte[] data)
        {
            if (!UseAdditionalEntropy || _additionalEntropy == null || data.Length == 0)
                return;
            int pos = _entropyIndex;
            var entropy = _additionalEntropy;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= entropy[pos % 32];
                pos++;
            }
            entropy = Sha256(data);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= entropy[pos % 32];
                pos++;
            }
            _entropyIndex = pos % 32;
        }

        /// <summary>
        /// The additional entropy.
        /// </summary>
        private static volatile byte[] _additionalEntropy = null;

        /// <summary>
        /// The entropy index..
        /// </summary>
        private static volatile int _entropyIndex = 0;

        /// <summary>
        /// Add entropy to the given data.
        /// </summary>
        /// <param name="data">The data to add entropy to.</param>
        /// <exception cref="ArgumentNullException">Thrown if the data array is null.</exception>
        public static void AddEntropy(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var entropy = Sha256(data);
            if (_additionalEntropy == null)
                _additionalEntropy = entropy;
            else
            {
                for (int i = 0; i < 32; i++)
                {
                    _additionalEntropy[i] ^= entropy[i];
                }
                _additionalEntropy = Sha256(_additionalEntropy);
            }
        }
        
        /// <summary>
        /// Calculates the Sha256 of the given data.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <returns>The hash.</returns>
        public static byte[] Sha256(byte[] data)
        {
            return Sha256(data, 0, data.Length);
        }

        /// <summary>
        /// Calculates the SHA256 of the given data.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <param name="offset">The offset at which to start.</param>
        /// <param name="count">The number of bytes to in the array to use as data.</param>
        /// <returns>The hash.</returns>
        private static byte[] Sha256(byte[] data, int offset, int count)
        {
            var SHA256CHECKSUM = SHA256.Create();
            return SHA256CHECKSUM.ComputeHash(data.AsSpan(offset,count).ToArray());
        }
    }
}