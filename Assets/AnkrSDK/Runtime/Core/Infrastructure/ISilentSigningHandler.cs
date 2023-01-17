using System.Threading.Tasks;

namespace AnkrSDK.Core.Infrastructure
{
	public interface ISilentSigningHandler
	{
		ISilentSigningSessionHandler SessionHandler { get; }
		Task<string> RequestSilentSign(long timestamp, string chainId = null);
		Task DisconnectSilentSign();
		Task<string> SendSilentTransaction(string from, string to, string data = null, string value = null,
			string gas = null,
			string gasPrice = null, string nonce = null);
		Task<string> SilentSignMessage(string address, string message);
		bool IsSilentSigningActive();
	}
}