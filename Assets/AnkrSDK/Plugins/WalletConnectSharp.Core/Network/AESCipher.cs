using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AnkrSDK.WalletConnect.VersionShared.Utils;
using AnkrSDK.WalletConnectSharp.Core.Models;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Core.Network
{
    public class AESCipher : ICipher
    {
        public async UniTask<EncryptedPayload> EncryptWithKey(byte[] key, string message, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            byte[] data = encoding.GetBytes(message);
            
            //Encrypt with AES/CBC/PKCS7Padding
            using (MemoryStream ms = new MemoryStream())
            {
                using (AesManaged ciphor = new AesManaged())
                {
                    ciphor.Mode = CipherMode.CBC;
                    ciphor.Padding = PaddingMode.PKCS7;
                    ciphor.KeySize = 256;

                    byte[] iv = ciphor.IV;

                    using (CryptoStream cs = new CryptoStream(ms, ciphor.CreateEncryptor(key, iv),
                        CryptoStreamMode.Write))
                    {
                        await cs.WriteAsync(data, 0, data.Length);
                    }

                    byte[] encryptedContent = ms.ToArray();

                    using (HMACSHA256 hmac = new HMACSHA256(key))
                    {
                        hmac.Initialize();
                        
                        byte[] toSign = new byte[iv.Length + encryptedContent.Length];
                        
                        //copy our 2 array into one
                        Buffer.BlockCopy(encryptedContent, 0, toSign, 0,encryptedContent.Length);
                        Buffer.BlockCopy(iv, 0, toSign, encryptedContent.Length, iv.Length);
                        
                        byte[] signature = hmac.ComputeHash(toSign);
                        
                        string ivHex = iv.ToHex();
                        string dataHex = encryptedContent.ToHex();
                        string hmacHex = signature.ToHex();
                        
                        return new EncryptedPayload()
                        {
                            data = dataHex,
                            hmac = hmacHex,
                            iv = ivHex
                        };
                    }
                }
            }
        }

        public async UniTask<string> DecryptWithKey(byte[] key, EncryptedPayload encryptedData, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            
            var rawData = encryptedData.data.HexToByteArray();
            var iv = encryptedData.iv.HexToByteArray();
            var hmacReceived = encryptedData.hmac.HexToByteArray();

            using (var hmac = new HMACSHA256(key))
            {
                hmac.Initialize();

                var toSign = new byte[iv.Length + rawData.Length];
                        
                //copy our 2 array into one
                Buffer.BlockCopy(rawData, 0, toSign, 0,rawData.Length);
                Buffer.BlockCopy(iv, 0, toSign, rawData.Length, iv.Length);
                
                var signature = hmac.ComputeHash(toSign);

                if (!signature.SequenceEqual(hmacReceived))
                    throw new InvalidDataException("HMAC Provided does not match expected"); //Ignore
            }

            using (var cryptor = new AesManaged())
            {
                cryptor.Mode = CipherMode.CBC;
                cryptor.Padding = PaddingMode.PKCS7;
                cryptor.KeySize = 256;

                cryptor.IV = iv;
                cryptor.Key = key;

                var decryptor = cryptor.CreateDecryptor(cryptor.Key, cryptor.IV);

                using (var ms = new MemoryStream(rawData))
                {
                    using (var sink = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            var read = 0;
                            var buffer = new byte[1024];
                            do
                            {
                                read = await cs.ReadAsync(buffer, 0, buffer.Length);
                                
                                if (read > 0)
                                    await sink.WriteAsync(buffer, 0, read);
                            } while (read > 0);

                            await cs.FlushAsync();

                            return encoding.GetString(sink.ToArray());
                        }
                    }
                }
            }
        }
    }
}