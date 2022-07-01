using System;
using System.Collections.Generic;
using System.Linq;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using TMPro;
using UnityEngine;

namespace AnkrSDK.UseCases.WebGlLogin
{
	public class WebGLLoginPanelController : MonoBehaviour
	{		
		[SerializeField]
		private GameObject _panel;
		
		[SerializeField]
		private WalletButton[] _buttons;

		[SerializeField]
		private TMP_Dropdown _dropdown;

		private List<string> _networks;

		public EventHandler<WebGL.SupportedWallets> WalletHasChosen;
		public EventHandler<NetworkName> NetworkHasChosen;

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
				button.OnClickHandler += OnWalletClick;
			}
		}
		
		private void DisableButtons()
		{
			foreach (var button in _buttons)
			{
				button.OnClickHandler -= OnWalletClick;
			}
		}

		private void OnChangeNetwork(int index)
		{
			var network = (NetworkName) Enum.Parse(typeof(NetworkName), _networks[index], true);
			NetworkHasChosen?.Invoke(this, network);
		}

		private void OnWalletClick(object sender, WebGL.SupportedWallets wallet)
		{
			WalletHasChosen?.Invoke(this, wallet);
		}

		public void ShowPanel()
		{
			_panel.SetActive(true);
		}
		
		public void HidePanel()
		{
			_panel.SetActive(false);
		}

		private void OnDisable()
		{
			DisableButtons();
			_dropdown.onValueChanged.RemoveListener(OnChangeNetwork);
		}
	}
}