using System;
using MirageSDK.WalletConnect.VersionShared.Models.DeepLink;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.UI
{
	public class WalletButton : MonoBehaviour
	{
		[SerializeField] private Image _image;
		[SerializeField] private TMP_Text _text;
		[SerializeField] private Button _button;
		public void Setup(WalletEntry walletData)
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