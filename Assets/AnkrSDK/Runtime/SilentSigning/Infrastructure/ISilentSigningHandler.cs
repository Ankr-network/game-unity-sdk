using System.Threading.Tasks;
using AnkrSDK.SilentSigning.Data.Responses;

namespace AnkrSDK.SilentSigning.Infrastructure
{
	public interface ISilentSigningHandler
	{
		Task<SilentSigningResponse> RequestSilentSign(long timestamp);
		Task DisconnectSilentSign();

		Task SendSilentTransaction(
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