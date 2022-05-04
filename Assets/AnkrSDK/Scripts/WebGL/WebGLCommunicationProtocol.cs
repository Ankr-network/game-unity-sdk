using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace AnkrSDK.WebGL
{
	public class WebGLCommunicationProtocol
	{
		private static Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>> _completionSources;
		private bool _isCancellationRequested = false;
		
		public WebGLCommunicationProtocol() {
			_completionSources = new Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>>();
		}

		public async UniTaskVoid Connect()
		{
			StartReceiveCycle();
		}
	
		public async UniTaskVoid StartReceiveCycle()
		{
			while (!_isCancellationRequested)
			{
				ReceiveMessages();

				await UniTask.Yield(PlayerLoopTiming.Update);
			}
		}
		
		public string GenerateId()
		{
			var id = Guid.NewGuid().ToString();
			
			var completionTask = new UniTaskCompletionSource<WebGLMessageDTO>();
			_completionSources.Add(id, completionTask);
			
			return id;
		}

		public async UniTask<WebGLMessageDTO> WaitForAnswer(string id)
		{
			var completionSource = _completionSources[id];
			var answer = await completionSource.Task;
			_completionSources.Remove(id);

			return answer;
		}

		private void ReceiveMessages()
		{
			var json = WebGLInterlayer.GetResponses();
			if (json.Length > 0)
			{
				var messages = JsonConvert.DeserializeObject<WebGLMessageDTO[]>(json);

				foreach (var message in messages)
				{
					if (_completionSources.ContainsKey(message.id))
					{
						var completionSource = _completionSources[message.id];
						completionSource.TrySetResult(message);
					}
				}
			}
		}

		public void Disconnect()
		{
			_isCancellationRequested = true;
			if (_completionSources.Count > 0)
			{
				CompleteAllSources();
			}

			_completionSources = null;
		}

		private void CompleteAllSources()
		{
			foreach(KeyValuePair<string, UniTaskCompletionSource<WebGLMessageDTO>> entry in _completionSources)
			{
				var answer = new WebGLMessageDTO
				{
					id = entry.Key,
					status = WebGLMessageStatus.Error,
					payload = "Answer didn't get before protocol disconnected"
				};
				entry.Value.TrySetResult(answer);
			}
			_completionSources.Clear();
		}

		~WebGLCommunicationProtocol()
		{
			Disconnect();
		}
	}
}