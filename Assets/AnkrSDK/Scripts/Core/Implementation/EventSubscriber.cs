using System;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

namespace AnkrSDK.Core.Implementation
{
	public class EventSubscriber
	{
		private static string url = "wss://mainnet.infura.io/ws/v3/c75f2ce78a4a4b64aa1e9c20316fda3e";
		private WebSocket transport;

		public EventSubscriber()
		{
			transport = new WebSocket(url);
			
			transport.OnOpen += OnSocketOpen;
			transport.OnMessage += OnMessageReceived;
			transport.OnClose += OnClose;
			transport.OnError += OnError;

			StartAsync().ContinueWith(task => StartConnect());
		}

		public async Task StartAsync()
		{
			await transport.Connect();
		}

		public Task StartConnect()
		{
			var message = "{\"id\":\"c672f27d-5bdc-4762-9db3-de7cb16a8015\",\"jsonrpc\":\"2.0\",\"method\":\"eth_subscribe\",\"params\":[\"logs\",{\"topics\":[\"0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef\"]}]}";
			var requestBytes = Encoding.UTF8.GetBytes(message);
			
			return transport.SendText(message);
		}

		private void OnSocketOpen()
		{
			Debug.Log("----- Socket opened -----");
		}

		private void OnMessageReceived(byte[] data)
		{
			Debug.Log("----- OnMessageReceived -----");
			Debug.Log(Encoding.UTF8.GetString(data));
		}

		private void OnError(string errorMesssage)
		{
			Debug.Log("----- !!! Error !!! -----");
		}

		private void OnClose(WebSocketCloseCode code)
		{
			Debug.Log($"----- Socket closed ({code}) -----");
		}
	}
}