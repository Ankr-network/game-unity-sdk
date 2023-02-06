using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UI
{
	public class WalletButton : MonoBehaviour
	{
		[SerializeField] private Image _image;
		[SerializeField] private TMP_Text _text;
		[SerializeField] private Button _button;
		public void Setup(AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink.AppEntry walletData)
		{
			_image.sprite = walletData.MediumIcon;
			_text.text = walletData.name;
		}

		public void SetListener(Action action)
		{
			_button.onClick.AddListener(() => action?.Invoke());
		}
	}
}