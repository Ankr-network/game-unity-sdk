using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;

namespace AnkrSDK.SilentSigning.Infrastructure
{
	public interface ISilentSigningHandler
	{
		bool IsSilentSigningActive();
		Task<string> RequestSilentSign(long timestamp);
		Task DisconnectSilentSign();
		Task<string> SendSilentTransaction(TransactionData transaction);
		Task<string> MakeSilentSignMessage(string address, string message);
	}
}