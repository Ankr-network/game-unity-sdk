using System;
using AnkrSDK.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.Examples.UseCases.WebGlLogin
{
	public class WalletButton : MonoBehaviour
	{
		[SerializeField]
		private WalletItem _walletItem;
		
		[SerializeField]
		private Image _logoContainer;
		
		[SerializeField]
		private TMP_Text _nameContainer;
		
		[SerializeField]
		private Button _button;
		
		public Action<Wallet> OnClickHandler;

		private void Start()
		{
			_logoContainer.sprite = _walletItem.Logo;
			_nameContainer.text = _walletItem.name;
			
			_button.onClick.AddListener(HandleClick);
		}

		private void HandleClick()
		{
			OnClickHandler?.Invoke(_walletItem.Type);
		}

		private void OnDisable()
		{
			_button.onClick.RemoveListener(HandleClick);
		}
	}
}