using Cysharp.Threading.Tasks;

namespace AnkrSDK.Core.Infrastructure
{
	public interface ISilentSigningHandler
	{
		ISilentSigningSessionHandler SessionHandler { get; }
		UniTask<string> RequestSilentSign(long timestamp, long chainId = 1);
		UniTask DisconnectSilentSign();
		UniTask<string> SendSilentTransaction(string from, string to, string data = null, string value = null,
			string gas = null,
			string gasPrice = null, string nonce = null);
		UniTask<string> SilentSignMessage(string address, string message);
		bool IsSilentSigningActive();
	}
}