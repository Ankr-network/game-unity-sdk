using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AnkrSDK.WalletConnect.VersionShared.Utils;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Network;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Unity.Network
{
    public class WebGlAESCipher : ICipher
    {
        public UniTask<EncryptedPayload> EncryptWithKey(byte[] key, string message, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var data = encoding.GetBytes(message);

            //Encrypt with AES/CBC/PKCS7Padding
            using (var ms = new MemoryStream())
            {
                using (var cipher = new AesManaged())
                {
                    cipher.Mode = CipherMode.CBC;
                    cipher.Padding = PaddingMode.PKCS7;
                    cipher.KeySize = 256;
                    
                    var iv = cipher.IV;

                    using (var cs = new CryptoStream(ms, cipher.CreateEncryptor(key, iv),
                        CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }

                    var encryptedContent = ms.ToArray();

                    using (var hmac = new HMACSHA256(key))
                    {
                        hmac.Initialize();
                        
                        var toSign = new byte[iv.Length + encryptedContent.Length];

                        //copy our 2 array into one
                        Buffer.BlockCopy(encryptedContent, 0, toSign, 0,encryptedContent.Length);
                        Buffer.BlockCopy(iv, 0, toSign, encryptedContent.Length, iv.Length);

                        var signature = hmac.ComputeHash(toSign);
                        
                        var ivHex = iv.ToHex();
                        var dataHex = encryptedContent.ToHex();
                        var hmacHex = signature.ToHex();

                        return UniTask.FromResult(new EncryptedPayload()
                        {
                            data = dataHex,
                            hmac = hmacHex,
                            iv = ivHex
                        });
                    }
                }
            }
        }

        public UniTask<string> DecryptWithKey(byte[] key, EncryptedPayload encryptedData, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

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
                {
                    throw new InvalidDataException("HMAC Provided does not match expected"); //Ignore
                }
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
                                read = cs.Read(buffer, 0, buffer.Length);
                                
                                if (read > 0)
                                {
                                    sink.Write(buffer, 0, read);
                                }
                            } while (read > 0);

                            cs.Flush();

                            return UniTask.FromResult(encoding.GetString(sink.ToArray()));
                        }
                    }
                }
            }
        }
    }
}