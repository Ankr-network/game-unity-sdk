using System;
using System.Collections.Generic;
using System.Linq;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AnkrSDK.UseCases.WebGlLogin
{
	public class WebGLLoginPanelController : MonoBehaviour
	{
		[Serializable]
		public class PanelEventWithWallet : UnityEvent<WebGL.SupportedWallets>{}
		
		[Serializable]
		public class PanelEventWithNetwork : UnityEvent<NetworkName>{}
		
		[SerializeField]
		private GameObject _panel;
		
		[SerializeField]
		private WalletButton[] _buttons;

		[SerializeField]
		private TMP_Dropdown _dropdown;

		private List<string> _networks;

		public PanelEventWithWallet WalletHasChosen;
		public PanelEventWithNetwork NetworkHasChosen;

		private void Start()
		{
			SetUpNetworkDropdown();
			SetUpButtons();
		}

		private void SetUpNetworkDropdown()
		{
			_networks = EthereumNetworks.Dictionary.Keys.Select(name => name.ToString()).ToList();
			_dropdown.AddOptions(_networks);
			_dropdown.onValueChanged.AddListener(OnChangeNetwork);
			_dropdown.value = _networks.FindIndex(network => network == NetworkName.Rinkeby.ToString());
		}
		
		private void SetUpButtons()
		{
			foreach (var button in _buttons)
			{
				button.OnClickHandler.AddListener(OnWalletClick);
			}
		}

		private void OnChangeNetwork(int index)
		{
			var network = (NetworkName) Enum.Parse(typeof(NetworkName), _networks[index], true);
			NetworkHasChosen?.Invoke(network);
		}

		private void OnWalletClick(WebGL.SupportedWallets wallet)
		{
			WalletHasChosen?.Invoke(wallet);
		}

		public void ShowPanel()
		{
			_panel.SetActive(true);
		}
		
		public void HidePanel()
		{
			_panel.SetActive(false);
		}
	}
}