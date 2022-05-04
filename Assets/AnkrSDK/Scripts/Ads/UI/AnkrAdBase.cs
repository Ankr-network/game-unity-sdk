using AnkrAds.Ads;
using AnkrAds.Ads.Data;
using Cysharp.Threading.Tasks;
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

		public virtual async UniTask SetupAd(AdData adData)
		{
			IsReady = false;
			var texture = await DownloadTexture(adData);
			OnTextureLoaded(texture);
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

		protected abstract void OnTextureLoaded(Sprite texture);

		private static UniTask<Sprite> DownloadTexture(AdData adData)
		{
			return AdsWebHelper.GetImageFromURL(adData.TextureURL).AsUniTask();
		}
	}
}