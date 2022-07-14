using AnkrSDK.Data;
using UnityEngine;

namespace AnkrSDK.Examples.UseCases.WebGlLogin
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
			_webGlLoginManager.NetworkChosen += OnNetworkChosen;
			_webGlLoginManager.WalletChosen += OnWalletChosen;
		}

		private void ActivatePanel()
		{
			_webGlLoginManager.ShowPanel();
		}

		private void ChangeLoginPanel(WebGL.WebGLWrapper provider)
		{
			_webGlLoginManager.HidePanel();
			_sceneChooser.SetActive(true);
		}

		private void OnNetworkChosen(NetworkName network)
		{
			_webGlConnect.SetNetwork(network);
		}
		
		private void OnWalletChosen(WebGL.SupportedWallets wallet)
		{
			_webGlConnect.SetWallet(wallet);
		}

		private void OnDisable()
		{
			_webGlConnect.OnNeedPanel -= ActivatePanel;
			_webGlConnect.OnConnect -= ChangeLoginPanel;
			_webGlLoginManager.NetworkChosen -= OnNetworkChosen;
			_webGlLoginManager.WalletChosen -= OnWalletChosen;
		}
	}
}