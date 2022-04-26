using System;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.Ads.UI
{
	public class AnkrBannerAdImage : AnkrAdBase
	{
		[SerializeField] private Image _image;
		[SerializeField] private Button _button;

		public event Action AdClicked;

		protected override void OnTextureLoaded(Sprite texture)
		{
			_image.sprite = texture;
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