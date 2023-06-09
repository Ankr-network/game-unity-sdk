using MirageSDK.WalletConnectSharp.Unity.Network.Client.Data;
using MirageSDK.WalletConnectSharp.Unity.Network.Client.EventHandlers;
using Cysharp.Threading.Tasks;

namespace MirageSDK.WalletConnectSharp.Unity.Network.Client.Infrastructure
{
	public interface IWebSocket
	{
		event WebSocketOpenEventHandler OnOpen;
		event WebSocketMessageEventHandler OnMessage;
		event WebSocketErrorEventHandler OnError;
		event WebSocketCloseEventHandler OnClose;

		WebSocketState State { get; }

		UniTask Connect();
		void DispatchMessageQueue();
		UniTask SendText(string requestMessage);
		UniTask Close();
		void CancelConnection();
	}
}