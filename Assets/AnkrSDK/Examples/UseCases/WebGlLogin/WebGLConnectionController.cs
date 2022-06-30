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
			
			_webGlConnect.OnNeedPanel.AddListener(ActivatePanel);
			_webGlConnect.OnConnect.AddListener(ChangeLoginPanel);
			_webGlLoginManager.NetworkHasChosen.AddListener(SetNetworkWhenItChosen);
			_webGlLoginManager.WalletHasChosen.AddListener(SetWalletWhenItChosen);
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

		private void SetNetworkWhenItChosen(NetworkName network)
		{
			_webGlConnect.SetNetwork(network);
		}
		
		private void SetWalletWhenItChosen(WebGL.SupportedWallets wallet)
		{
			_webGlConnect.SetWallet(wallet);
		}
	}
}