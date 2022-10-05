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

		protected override void OnTextureLoaded(Texture2D texture)
		{
			var sprite = Sprite.Create(texture,  new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			_image.sprite = sprite;
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