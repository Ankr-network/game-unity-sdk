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

		public void SetupAd(AdData adData)
		{
			_sprite.sprite = adData.Sprite;
			_sprite.size = adData.Size;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			AdClicked?.Invoke();
		}
	}
}