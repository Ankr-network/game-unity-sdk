using System;
using AnkrAds.Ads.Infrastructure;
using UnityEngine;

namespace AnkrSDK.Ads.IOS
{
	public class IOSCallbackListener : IAdsEventsListener
	{
		public event Action AdInitialized;
		public event Action AdOpened;
		public event Action AdClosed;
		public event Action AdFinished;
		public event Action<string> AdLoaded;
		public event Action<string> AdFailedToLoad;
		public event Action<string> AdClicked;
		public event Action<string> AdRewarded;
		public event Action<string> Error;
		public event Action<string, byte[]> AdTextureReceived;

		public void ProcessEventMessage(string message)
		{
			var splitMessage = message.Split(';');
			var methodName = splitMessage[0];
			switch (methodName)
			{
				case "Loaded":
				{
					AdLoaded?.Invoke(splitMessage[1]);
					break;
				}
				case "FailedToLoad":
				{
					AdFailedToLoad?.Invoke(splitMessage[1]);
					break;
				}
				case "Clicked":
				{
					AdClicked?.Invoke(splitMessage[1]);
					break;
				}
				case "Opened":
				{
					AdOpened?.Invoke();
					break;
				}
				case "Closed":
				{
					AdClosed?.Invoke();
					break;
				}
				case "Finished":
				{
					AdFinished?.Invoke();
					break;
				}
				case "LoadedImage":
				{
					var base64String = splitMessage[2];
					var bytes = Convert.FromBase64String(base64String);
					AdTextureReceived?.Invoke(splitMessage[1], bytes);
					break;
				}
				case "LoadedVideoURL":
				{
					Debug.Log($"Received video:{splitMessage[1]} saved at {splitMessage[2]}");
					break;
				}
				case "Error":
				{
					Error?.Invoke(splitMessage[1]);
					Debug.LogError($"Error in ads:{splitMessage[1]}");
					break;
				}
				case "Initialised":
				{
					AdInitialized?.Invoke();
					break;
				}
			}
		}
	}
}