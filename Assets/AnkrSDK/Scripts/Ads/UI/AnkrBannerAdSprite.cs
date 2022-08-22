using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AnkrSDK.Ads.UI
{
	public class AnkrBannerAdSprite : AnkrAdBase, IPointerClickHandler
	{
		[SerializeField] private SpriteRenderer _sprite;
		[SerializeField] private Vector2 _spriteSize;

		public event Action AdClicked;

		public void OnPointerClick(PointerEventData eventData)
		{
			AdClicked?.Invoke();
		}

		protected override void OnTextureLoaded(Texture2D texture)
		{
			var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
				new Vector2(0.5f, 0.5f));
			_sprite.sprite = sprite;
			_sprite.size = new Vector2(_spriteSize.x, _spriteSize.y);
		}
	}
}