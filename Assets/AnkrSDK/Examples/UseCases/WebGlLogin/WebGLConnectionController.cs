using System;
using AnkrSDK.Data;
using UnityEngine;

namespace AnkrSDK.UseCases.WebGlLogin
{
	public class WebGLConnectionController : MonoBehaviour
	{
		[SerializeField]
		private WebGl.WebGLConnect _webGlConnect;

		[SerializeField]
		private WebGLLoginPanelController _webGlLoginManager;
		
		[SerializeField]
		private GameObject _sceneChooser;

		private void Start()
		{
			_sceneChooser.SetActive(false);
			
			_webGlConnect.OnNeedPanel += ActivatePanel;
			_webGlConnect.OnConnect += ChangeLoginPanel;
			_webGlLoginManager.NetworkHasChosen += OnNetworkChosen;
			_webGlLoginManager.WalletHasChosen += OnWalletChosen;
		}

		private void ActivatePanel(object sender, EventArgs empty)
		{
			_webGlLoginManager.ShowPanel();
		}

		private void ChangeLoginPanel(object sender, WebGL.WebGLWrapper provider)
		{
			_webGlLoginManager.HidePanel();
			_sceneChooser.SetActive(true);
		}

		private void OnNetworkChosen(object sender, NetworkName network)
		{
			_webGlConnect.SetNetwork(network);
		}
		
		private void OnWalletChosen(object sender, WebGL.SupportedWallets wallet)
		{
			_webGlConnect.SetWallet(wallet);
		}

		private void OnDisable()
		{
			_webGlConnect.OnNeedPanel -= ActivatePanel;
			_webGlConnect.OnConnect -= ChangeLoginPanel;
			_webGlLoginManager.NetworkHasChosen -= OnNetworkChosen;
			_webGlLoginManager.WalletHasChosen -= OnWalletChosen;
		}
	}
}