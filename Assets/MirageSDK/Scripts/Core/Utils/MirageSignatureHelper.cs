using System.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Unity;
using Nethereum.Signer;

namespace MirageSDK.Core.Utils
{
	public static class MirageSignatureHelper
	{
		/// <summary>
		/// Sign a message using  currently active session.
		/// </summary>
		/// <param name="messageToSign">Message you would like to sign</param>
		/// <returns>Signed message</returns>
		public static Task<string> Sign(string messageToSign)
		{
			return WalletConnect.ActiveSession.EthSign(WalletConnect.ActiveSession.Accounts[0], messageToSign);
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