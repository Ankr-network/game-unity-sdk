using System;
using System.Collections.Generic;
using System.Linq;
using MirageSDK.Data;
using MirageSDK.Utils;
using TMPro;
using UnityEngine;

namespace MirageSDK.Examples.UseCases.WebGlLogin
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

		public Action<Wallet> WalletChosen;
		public Action<NetworkName> NetworkChosen;

		private void Start()
		{
			SetUpNetworkDropdown();
			SetUpButtons();
		}

		private void SetUpNetworkDropdown()
		{
			_networks = EthereumNetworks.AllAddedNetworks.Select(networkName => networkName.ToString()).ToList();
			_dropdown.AddOptions(_networks);
			_dropdown.onValueChanged.AddListener(OnChangeNetwork);
			_dropdown.value = _networks.FindIndex(network => network == NetworkName.Goerli.ToString());
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
			NetworkChosen?.Invoke(network);
		}

		private void OnWalletClick(Wallet wallet)
		{
			WalletChosen?.Invoke(wallet);
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