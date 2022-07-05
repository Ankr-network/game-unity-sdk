using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.WebGlLogin
{
	[RequireComponent(typeof(Button))]
	public class WalletButton : MonoBehaviour
	{
		[SerializeField]
		private WalletItem _walletItem;
		
		[SerializeField]
		private Image _logoContainer;
		
		[SerializeField]
		private TMP_Text _nameContainer;
		
		[SerializeField]
		public EventHandler<WebGL.SupportedWallets> OnClickHandler;

		private Button _button;

		private void Start()
		{
			_logoContainer.sprite = _walletItem.Logo;
			_nameContainer.text = _walletItem.name;
			
			_button = GetComponent<Button>();
			_button.onClick.AddListener(HandleClick);
		}

		private void HandleClick()
		{
			OnClickHandler?.Invoke(this, _walletItem.Type);
		}

		private void OnDisable()
		{
			_button.onClick.RemoveListener(HandleClick);
		}
	}
}