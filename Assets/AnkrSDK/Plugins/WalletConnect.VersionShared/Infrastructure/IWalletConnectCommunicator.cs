using System.Numerics;
using System.Text;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum.Types;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnect.VersionShared.Infrastructure
{
	public interface IWalletConnectCommunicator
	{
		UniTask<string> EthSign(string address, string message);
		UniTask<string> EthPersonalSign(string address, string message);
		UniTask<string> EthSignTypedData<T>(string address, T data, EIP712Domain eip712Domain);
		UniTask<string> EthSendTransaction(params TransactionData[] transaction);
		UniTask<string> EthSignTransaction(params TransactionData[] transaction);
		UniTask<string> EthSendRawTransaction(string data, Encoding messageEncoding = null);
		UniTask<string> WalletAddEthChain(EthChainData chainData);
		UniTask<string> WalletSwitchEthChain(EthChain chainData);
		UniTask<string> WalletUpdateEthChain(EthUpdateChainData chainData);
		UniTask<BigInteger> EthChainId();
		string GetDefaultAccount(string network);

		UniTask<TResponse> Send<TRequest, TResponse>(TRequest data)
			where TRequest : IIdentifiable
			where TResponse : IErrorHolder;
	}
}