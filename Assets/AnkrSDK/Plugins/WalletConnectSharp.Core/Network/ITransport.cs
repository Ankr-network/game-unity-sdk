using System;
using AnkrSDK.WalletConnectSharp.Core.Models;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Core.Network
{
    public interface ITransport : IDisposable
    {
        bool Connected { get; }
        
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<MessageReceivedEventArgs> OpenReceived;
        event EventHandler<MessageReceivedEventArgs> Closed;
        
        string URL { get; }
        
        UniTask Open(string bridgeURL, bool clearSubscriptions = true);

        UniTask Close();

        UniTask SendMessage(NetworkMessage message);

        UniTask Subscribe(string topic);

        void ClearSubscriptions();
    }
}