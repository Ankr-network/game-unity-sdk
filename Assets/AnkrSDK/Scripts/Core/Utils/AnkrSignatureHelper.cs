using System.Threading.Tasks;
using AnkrSDK.Core.Implementation;
using AnkrSDK.WalletConnectSharp.Unity;
using AnkrSDK.WebGL;
using Nethereum.Signer;

namespace AnkrSDK.Core.Utils
{
	public class AnkrSignatureHelper
	{
		
		/// <summary>
		/// Sign a message using  currently active session.
		/// </summary>
		/// <param name="messageToSign">Message you would like to sign</param>
		/// <returns>Signed message</returns>
		public static async Task<string> Sign(string messageToSign, string address)
		{
#if UNITY_WEBGL
			var props = new DataSignaturePropsDTO
			{
				address = address,
				message = messageToSign
			};
			var interlayer = WebGLWrapper.Instance();
			return await interlayer.Sign(props);
#else
			return await WalletConnect.ActiveSession.EthSign(await EthHandler.GetDefaultAccount(), messageToSign);
#endif
		}

		/// <summary>
		/// Checks if message was signed with provided <paramref name="signature"/>
		/// For more info look into Netherium.Signer implementation.
		/// </summary>
		/// <param name="messageToCheck"></param>
		/// <param name="signature"></param>
		/// <returns>Messages public address.</returns>
		public static string CheckSignature(string messageToCheck, string signature)
		{
			var signer = new EthereumMessageSigner();
			return signer.EncodeUTF8AndEcRecover(messageToCheck, signature);
		}
	}
}