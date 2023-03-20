using Nethereum.Signer;

namespace MirageSDK.Core.Utils
{
	public static class MirageSignatureHelper
	{
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