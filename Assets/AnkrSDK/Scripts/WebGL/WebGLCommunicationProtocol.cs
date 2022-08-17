using System;
using System.Collections.Generic;
using System.Threading;
using AnkrSDK.WebGL.DTO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

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
			Debug.Log("----------- GenerateId -----------");
			var id = Guid.NewGuid().ToString();
			
			var completionTask = new UniTaskCompletionSource<WebGLMessageDTO>();
			_completionSources.Add(id, completionTask);
			
			Debug.Log(JsonConvert.SerializeObject(_completionSources));
			
			return id;
		}

		public async UniTask<WebGLMessageDTO> WaitForAnswer(string id)
		{
			Debug.Log("----------- WaitForAnswer -----------");
			var completionTask = _completionSources[id];
			Debug.Log("ids = " + JsonConvert.SerializeObject(_completionSources));
			var answer = await completionTask.Task;
			_completionSources.Remove(id);
			
			return answer;
		}

		public void Disconnect()
		{
			Debug.Log("----------- Disconnect -----------");
			_cancellationTokenSource.Cancel();
			if (_completionSources.Count > 0)
			{
				CompleteAllSources();
			}
		}

		private void ReceiveMessages()
		{
			var json = WebGLInterlayer.GetResponses();
			Debug.Log(json);
			if (json.Length <= 0)
			{
				return;
			}

			var messages = JsonConvert.DeserializeObject<WebGLMessageDTO[]>(json);

			foreach (var message in messages)
			{
				Debug.Log("||||| 1 |||||");
				Debug.Log("message.id = " + message.id);
				Debug.Log("ids = " + JsonConvert.SerializeObject(_completionSources));
				Debug.Log("contains id = " + _completionSources.ContainsKey(message.id));
				if (_completionSources.ContainsKey(message.id))
				{
					Debug.Log("||||| 2 |||||");
					var completionSource = _completionSources[message.id];
					completionSource.TrySetResult(message);
				}
			}
		}

		private void CompleteAllSources()
		{
			Debug.Log("----------- CompleteAllSources -----------");
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
