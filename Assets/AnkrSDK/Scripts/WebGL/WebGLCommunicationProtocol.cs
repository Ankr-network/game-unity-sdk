#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace AnkrSDK.WebGL
{
	public class WebGLCommunicationProtocol
	{
		private Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>> _completionSources;
		private bool _isCancellationRequested;

		public WebGLCommunicationProtocol()
		{
			_completionSources = new Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>>();
		}

		public UniTask Connect()
		{
			return StartReceiveCycle();
		}

		private async UniTask StartReceiveCycle()
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

		public void Disconnect()
		{
			_isCancellationRequested = true;
			if (_completionSources.Count > 0)
			{
				CompleteAllSources();
			}

			_completionSources = null;
		}

		private void ReceiveMessages()
		{
			var json = WebGLInterlayer.GetResponses();
			if (json.Length <= 0)
			{
				return;
			}

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

		private void CompleteAllSources()
		{
			foreach (var entry in _completionSources)
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
#endif