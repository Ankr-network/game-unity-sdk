using UnityEngine;

namespace AnkrSDK.Ads.UI
{
	public abstract class AnkrAdBase : MonoBehaviour
	{
		private bool _isReady;

		public bool IsReady
		{
			get => _isReady;
			private set
			{
				if (!value)
				{
					gameObject.SetActive(false);
				}

				_isReady = value;
			}
		}

		public void SetupAd(Texture2D texture2D)
		{
			IsReady = false;
			OnTextureLoaded(texture2D);
			IsReady = true;
		}

		public void TryShow()
		{
			if (!_isReady)
			{
				Debug.LogError("Trying to show Ad which is not ready yet.");
				return;
			}

			gameObject.SetActive(true);
		}

		protected abstract void OnTextureLoaded(Texture2D texture);
	}
}