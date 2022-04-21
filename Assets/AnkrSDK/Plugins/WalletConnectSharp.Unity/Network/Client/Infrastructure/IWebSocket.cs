using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.EventHandlers;

namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client.Infrastructure
{
	public interface IWebSocket
	{
		event WebSocketOpenEventHandler OnOpen;
		event WebSocketMessageEventHandler OnMessage;
		event WebSocketErrorEventHandler OnError;
		event WebSocketCloseEventHandler OnClose;

		WebSocketState State { get; }

		Task Connect();
		void DispatchMessageQueue();
		Task SendText(string requestMessage);
		Task Close();
	}
}