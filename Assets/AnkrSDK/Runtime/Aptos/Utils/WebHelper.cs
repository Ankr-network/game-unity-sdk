using System;
using System.Collections.Generic;
using System.Text;
using AnkrSDK.Aptos.Converters;
using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkrSDK.Aptos
{
	public static class WebHelper
	{
		public static UniTask<TResultType> SendPostRequest<TPayloadType, TResultType>(
			string url,
			TPayloadType payload,
			Dictionary<string, string> headers = null
		)
		{
			Debug.Log(url);
			var payloadJson = JsonConvert.SerializeObject(payload);
			var webRequest = UnityWebRequest.Post(url, payloadJson);
			return SendChangeRequest<TResultType>(webRequest, payloadJson, headers);
		}

		public static UniTask<TResultType> SendPostRequest<TResultType>(
			string url,
			Dictionary<string, string> headers = null
		)
		{
			var webRequest = UnityWebRequest.Post(url, String.Empty);
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
			string wrapper = null
		)
		{
			Debug.Log(urlWithQuery);
			using var webRequest = UnityWebRequest.Get(urlWithQuery);
			if (headers != null)
			{
				AddHeaders(webRequest, headers);
			}

			webRequest.timeout = 5;

			var answer = await webRequest.SendWebRequest();

			var json = webRequest.downloadHandler.text;
			if (answer == UnityWebRequest.Result.Success)
			{
				try
				{
					var jsonPayload = json;
					if (wrapper != null)
					{
						jsonPayload = string.Format(wrapper, json);
					}
					var result = JsonConvert.DeserializeObject<TResultType>(jsonPayload);
					return result;
				}
				catch (Exception e)
				{
					Debug.LogError($"Error while deserializing response: {e.Message}");
					throw e;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(json))
				{
					Debug.LogError(webRequest.error);
					throw new InvalidOperationException(webRequest.error);
				}
				else
				{
					var error = JsonConvert.DeserializeObject<Error>(json);
					Debug.LogError(AptosException.CreateMessage(error.Message, error.ErrorCode, error.VmErrorCode));
					throw new AptosException(error.Message, error.ErrorCode, error.VmErrorCode);
				}
			}
		}

		private static async UniTask<TResultType> SendChangeRequest<TResultType>(
			UnityWebRequest request,
			string payloadJson = null,
			Dictionary<string, string> headers = null
		)
		{
			using (request)
			{
				request.disposeUploadHandlerOnDispose = true;
				if (headers != null)
				{
					AddHeaders(request, headers);
				}

				request.timeout = 5;

				if (payloadJson != null)
				{
					var webRequestUploadHandler = CreateUploadHandler(payloadJson);
					request.uploadHandler = webRequestUploadHandler;
				}

				var answer = await request.SendWebRequest();
				var json = request.downloadHandler.text;

				if (answer == UnityWebRequest.Result.Success)
				{
					try
					{
						var result = JsonConvert.DeserializeObject<TResultType>(json);
						return result;
					}
					catch (Exception e)
					{
						var message = $"Error while deserializing response: {e.Message}";
						Debug.LogError(message);
						throw new Exception(message);
					}
				}
				else
				{
					if (string.IsNullOrEmpty(json))
					{
						Debug.LogError(request.error);
						throw new InvalidOperationException(request.error);
					}
					else
					{
						var error = JsonConvert.DeserializeObject<Error>(json);
						Debug.LogError(AptosException.CreateMessage(error.Message, error.ErrorCode, error.VmErrorCode));
						throw new AptosException(error.Message, error.ErrorCode, error.VmErrorCode);
					}
				}
			}
		}

		private static void AddHeaders(UnityWebRequest request, Dictionary<string, string> headers)
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