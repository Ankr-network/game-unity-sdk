using System;
using MirageSDK.Plugins.WalletConnectSharp.Core.Models;

namespace MirageSDK.Plugins.WalletConnectSharp.Core.Network
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public NetworkMessage Message { get; private set; }
        
        public ITransport Source { get; private set; }

        public MessageReceivedEventArgs(NetworkMessage message, ITransport source)
        {
            Message = message;
            Source = source;
        }
    }
}