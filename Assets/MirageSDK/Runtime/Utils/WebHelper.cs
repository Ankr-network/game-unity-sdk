using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace MirageSDK.Utils
{
	public static class WebHelper
	{
		public static UniTask<TResultType> SendPostRequest<TPayloadType, TResultType>(
			string url,
			TPayloadType payload,
			Dictionary<string, string> headers = null
		)
		{
			var payloadJson = JsonConvert.SerializeObject(payload);

			Debug.Log($"Sending to {url}");
			Debug.Log($"Sending with {payloadJson}");
			var webRequest = UnityWebRequest.Post(url, payloadJson);
			return SendChangeRequest<TResultType>(webRequest, payloadJson, headers);
		}

		public static UniTask<TResultType> SendPostRequest<TResultType>(
			string url,
			Dictionary<string, string> headers = null
		)
		{
			var webRequest = UnityWebRequest.Post(url, string.Empty);
			return SendChangeRequest<TResultType>(webRequest, headers: headers);
		}

		public static UniTask<TResultType> SendPutRequest<TPayloadType, TResultType>(
			string url,
			TPayloadType payload,
			Dictionary<string, string> headers = null
		)
		{
			var payloadJson = JsonConvert.SerializeObject(payload);
			var webRequest = UnityWebRequest.Put(url, payloadJson);
			return SendChangeRequest<TResultType>(webRequest, payloadJson, headers);
		}

		public static async UniTask<TResultType> SendGetRequest<TResultType>(
			string urlWithQuery,
			Dictionary<string, string> headers = null,
			int timeout = 60
		)
		{
			using (var webRequest = UnityWebRequest.Get(urlWithQuery))
			{
				if (headers != null)
				{
					webRequest.AddHeaders(headers: headers);
				}

				webRequest.timeout = timeout;

				var answer = await webRequest.SendWebRequest();

				var requestResult = answer.result;
				var json = webRequest.downloadHandler.text;
				if (requestResult == UnityWebRequest.Result.Success)
				{
					try
					{
						var result = JsonConvert.DeserializeObject<TResultType>(json);
						return result;
					}
					catch (Exception e)
					{
						Debug.LogError($"Error while deserializing response: {e.Message}");
						throw;
					}
				}

				Debug.LogError(webRequest.error);
				throw new Exception(webRequest.error);
			}
		}

		private static async UniTask<TResultType> SendChangeRequest<TResultType>(
			UnityWebRequest request,
			string payloadJson = null,
			Dictionary<string, string> headers = null,
			int timeout = 60
		)
		{
			using (request)
			{
				request.disposeUploadHandlerOnDispose = true;
				if (headers != null)
				{
					request.AddHeaders(headers);
				}

				request.timeout = timeout;

				if (payloadJson != null)
				{
					var webRequestUploadHandler = CreateUploadHandler(payloadJson);
					request.uploadHandler = webRequestUploadHandler;
				}

				var answer = await request.SendWebRequest();
				var json = request.downloadHandler.text;

				return ParseJsonResponse<TResultType>(json, answer);
			}
		}

		public static async UniTask<TResultType> SendPostRequestURLEncoded<TResultType>(
			string url,
			Dictionary<string, string> payload,
			Dictionary<string, string> headers = null
		)
		{
			using (var webRequest = UnityWebRequest.Post(url, payload))
			{
				if (headers != null)
				{
					webRequest.AddHeaders(headers);
				}

				var answer = await webRequest.SendWebRequest();
				var json = webRequest.downloadHandler.text;

				return ParseJsonResponse<TResultType>(json, answer);
			}
		}

		public static async UniTask SendPostRequestURLEncoded(
			string url,
			Dictionary<string, string> payload,
			Dictionary<string, string> headers = null
		)
		{
			using (var webRequest = UnityWebRequest.Post(url, payload))
			{
				if (headers != null)
				{
					webRequest.AddHeaders(headers);
				}

				await webRequest.SendWebRequest();
			}
		}

		private static TResultType ParseJsonResponse<TResultType>(string json,
			UnityWebRequest request)
		{
			switch (request.result)
			{
				case UnityWebRequest.Result.Success:
					try
					{
						var result = JsonConvert.DeserializeObject<TResultType>(json);
						return result;
					}
					catch (Exception e)
					{
						Debug.LogError($"Error while deserialising response: {e.Message}");
						return default;
					}
				default:
					Debug.LogError($"{request.error}. \n {json}");
					return default;
			}
		}

		private static void AddHeaders(this UnityWebRequest request, Dictionary<string, string> headers)
		{
			foreach (var entryHeader in headers)
			{
				request.SetRequestHeader(entryHeader.Key, entryHeader.Value);
			}
		}

		private static UploadHandler CreateUploadHandler(string payload)
		{
			var payloadBytes = Encoding.UTF8.GetBytes(payload);
			var uploadHandler = new UploadHandlerRaw(payloadBytes);
			uploadHandler.contentType = "application/json";
			return uploadHandler;
		}
	}
}