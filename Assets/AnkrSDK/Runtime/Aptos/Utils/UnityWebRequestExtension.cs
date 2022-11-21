using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace AnkrSDK.Aptos
{
	public static class UnityWebRequestExtension
	{
		public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOperation)
		{
			var webRequestTask = new TaskCompletionSource<UnityWebRequest.Result>();
			reqOperation.completed += asyncOp => webRequestTask.TrySetResult(reqOperation.webRequest.result);

			if (reqOperation.isDone)
			{
				webRequestTask.TrySetResult(reqOperation.webRequest.result);
			}

			return webRequestTask.Task.GetAwaiter();
		}
	}
}