using System;
using System.Collections.Generic;
using System.Linq;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace AnkrSDK.WebGL
{
	public class WebGLWrapper
	{
		private static WebGLWrapper instance;
		
		private static Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>> completionSources;
		private bool _isCancellationRequested;

		protected WebGLWrapper()
		{
			completionSources = new Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>>();
			Update().Forget();
		}

		public static WebGLWrapper Instance()
		{
			if (instance == null)
			{
				instance = new WebGLWrapper();
			}

			return instance;
		}

		public async UniTaskVoid Update()
		{
			while (!_isCancellationRequested)
			{
				ReceiveMessages();

				await UniTask.Delay(100);
			}
		}
    
		public async UniTask<string> SendTransaction(TransactionData transaction)
		{
			var id = GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.SendTransaction(id, payload);
        
			var completionTask = new UniTaskCompletionSource<WebGLMessageDTO>();
			completionSources.Add(id, completionTask);

			var answer = await completionTask.Task;
			completionSources.Remove(id);

			return answer.payload;
		}

		public async UniTask<string> Sign(string message)
		{
			var id = GenerateId();
			WebGLInterlayer.SignMessage(id, message);
        
			var completionTask = new UniTaskCompletionSource<WebGLMessageDTO>();
			completionSources.Add(id, completionTask);

			var answer = await completionTask.Task;
			completionSources.Remove(id);

			return answer.payload;
		}
		
		public async UniTask<string> GetDefaultAccount()
		{
			var id = GenerateId();
			WebGLInterlayer.GetAddresses(id);
        
			var completionTask = new UniTaskCompletionSource<WebGLMessageDTO>();
			completionSources.Add(id, completionTask);

			var answer = await completionTask.Task;
			completionSources.Remove(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var addresses = JsonConvert.DeserializeObject<string[]>(answer.payload);
				return addresses.First();
			}

			throw new Exception(answer.payload);
		}

		private string GenerateId()
		{
			return Guid.NewGuid().ToString();
		}

		private void ReceiveMessages()
		{
			var json = WebGLInterlayer.GetResponses();
			var messages = JsonConvert.DeserializeObject<WebGLMessageDTO[]>(json);
			if (messages.Length > 0)
			{
				foreach (var message in messages)
				{
					if (completionSources.ContainsKey(message.id))
					{
						var completionSource = completionSources[message.id];
						completionSource.TrySetResult(message);
					}
				}
			}
		}
	}
}