using System;
using System.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Core.Events.Model;
using MirageSDK.WalletConnectSharp.Core.Models;

namespace MirageSDK.WalletConnectSharp.Core.Network
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

        Task Subscribe<T>(string topic, EventHandler<JsonRpcResponseEvent<T>> callback) where T : JsonRpcResponse;

        Task Subscribe<T>(string topic, EventHandler<JsonRpcRequestEvent<T>> callback) where T : JsonRpcRequest;

        void ClearSubscriptions();
    }
}