using System;
using System.Collections.Generic;
using System.Threading;
using AnkrSDK.WebGL.DTO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace AnkrSDK.WebGL
{
	internal class WebGLCommunicationProtocol
	{
		private readonly Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>> _completionSources =
			new Dictionary<string, UniTaskCompletionSource<WebGLMessageDTO>>();

		private CancellationTokenSource _cancellationTokenSource;

		~WebGLCommunicationProtocol()
		{
			Disconnect();
		}

		public async UniTaskVoid StartReceiveCycle()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			var token = _cancellationTokenSource.Token;
			while (!token.IsCancellationRequested)
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
			var completionTask = _completionSources[id];
			var answer = await completionTask.Task;
			_completionSources.Remove(id);

			return answer;
		}

		public void Disconnect()
		{
			_cancellationTokenSource.Cancel();
			if (_completionSources.Count > 0)
			{
				CompleteAllSources();
			}
		}

		private void ReceiveMessages()
		{
			var json = WebGLInterlayer.GetResponses();
			if (String.IsNullOrEmpty(json))
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
					payload = "Answer was not received before protocol disconnected"
				};
				entry.Value.TrySetResult(answer);
			}

			_completionSources.Clear();
		}
	}
}