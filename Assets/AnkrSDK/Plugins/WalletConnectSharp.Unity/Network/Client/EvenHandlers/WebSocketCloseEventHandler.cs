using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data;
#if UNITY_WEBGL && !UNITY_EDITOR
using AOT;
using System.Runtime.InteropServices;
#endif

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client.EvenHandlers
{
    public delegate void WebSocketCloseEventHandler(WebSocketCloseCode closeCode);
}