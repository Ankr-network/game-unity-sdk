using System;
using AnkrSDK.Ads.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AnkrSDK.Ads.UI
{
	public class AnkrBannerAdSprite : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private SpriteRenderer _sprite;

		public event Action AdClicked;

		public void SetupAd(Sprite sprite)
		{
			_sprite.sprite = sprite;
			_sprite.size = sprite.rect.size;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			AdClicked?.Invoke();
		}
	}
}