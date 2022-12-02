using System.Threading.Tasks;

namespace AnkrSDK.SilentSigning.Infrastructure
{
	public interface ISilentSigningHandler
	{
		bool IsSilentSigningActive();
		Task<string> RequestSilentSign(long timestamp);
		Task DisconnectSilentSign();
		Task<string> SendSilentTransaction(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		);
	}
}