using System;
using AnkrSDK.Ads.Data;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.Ads.UI
{
	public class AnkrBannerAdImage : MonoBehaviour
	{
		[SerializeField] private Image _image;
		[SerializeField] private Button _button;

		public event Action AdClicked;

		public void SetupAd(AdData adData)
		{
			_image.sprite = adData.Sprite;
			_image.SetNativeSize();
		}

		private void OnEnable()
		{
			_button.onClick.AddListener(OnButtonClicked);
		}

		private void OnDisable()
		{
			_button.onClick.RemoveAllListeners();
		}

		private void OnButtonClicked()
		{
			AdClicked?.Invoke();
		}
	}
}