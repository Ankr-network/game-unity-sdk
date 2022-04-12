using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AnkrSDK.Ads.UI
{
	public class AnkrBannerAdSprite : AnkrAdBase, IPointerClickHandler
	{
		[SerializeField] private SpriteRenderer _sprite;

		public event Action AdClicked;

		public void OnPointerClick(PointerEventData eventData)
		{
			AdClicked?.Invoke();
		}

		protected override void OnTextureLoaded(Sprite texture)
		{
			_sprite.sprite = texture;
			_sprite.size = texture.rect.size;
		}
	}
}