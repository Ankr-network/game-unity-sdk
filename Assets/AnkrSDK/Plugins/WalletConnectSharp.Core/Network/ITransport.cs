using System;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Events.Model;
using AnkrSDK.WalletConnectSharp.Core.Models;

namespace AnkrSDK.WalletConnectSharp.Core.Network
{
    public interface ITransport : IDisposable
    {
        bool Connected { get; }
        
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<MessageReceivedEventArgs> OpenReceived;
        event EventHandler<MessageReceivedEventArgs> Closed;
        
        string URL { get; }
        
        Task Open(string bridgeURL, bool clearSubscriptions = true);

        Task Close();

        Task SendMessage(NetworkMessage message);

        Task Subscribe(string topic);

        void ClearSubscriptions();
    }
}