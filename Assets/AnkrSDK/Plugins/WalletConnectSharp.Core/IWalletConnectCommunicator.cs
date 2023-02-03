using System.Text;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum.Types;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Core
{
    public interface IWalletConnectCommunicator
    {
        WalletConnectStatus Status { get; }
        UniTask<string> EthSign(string address, string message);
        UniTask<string> EthPersonalSign(string address, string message);
        UniTask<string> EthSignTypedData<T>(string address, T data, EIP712Domain eip712Domain);
        UniTask<string> EthSendTransaction(params TransactionData[] transaction);
        UniTask<string> EthSignTransaction(params TransactionData[] transaction);
        UniTask<string> EthSendRawTransaction(string data, Encoding messageEncoding = null);

        UniTask<TResponse> Send<TRequest, TResponse>(TRequest data) where TRequest : JsonRpcRequest
            where TResponse : JsonRpcResponse;
    }
}