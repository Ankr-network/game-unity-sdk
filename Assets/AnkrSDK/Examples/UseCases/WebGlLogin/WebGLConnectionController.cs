using AnkrSDK.Data;
using AnkrSDK.Utils;
using AnkrSDK.WebGL;
using UnityEngine;

namespace AnkrSDK.Examples.UseCases.WebGlLogin
{
	public class WebGLConnectionController : MonoBehaviour
	{
		[SerializeField]
		private WebGLLoginPanelController _webGlLoginManager;
		
		[SerializeField]
		private GameObject _sceneChooser;
		private WebGLConnect WebGLConnect => ConnectProvider<WebGLConnect, WebGLConnectSettingsSO>.GetConnect();

		private void Awake()
		{
			WebGLConnect.OnNeedPanel += ActivatePanel;
			WebGLConnect.OnConnect += ChangeLoginPanel;
			_webGlLoginManager.NetworkChosen += OnNetworkChosen;
			_webGlLoginManager.WalletChosen += OnWalletChosen;
		}

		private async void Start()
		{
			_sceneChooser.SetActive(false);
			await WebGLConnect.Connect();
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
			WebGLConnect.SetNetwork(network);
		}
		
		private void OnWalletChosen(Wallet wallet)
		{
			WebGLConnect.SetWallet(wallet);
		}

		private void OnDisable()
		{
			WebGLConnect.OnNeedPanel -= ActivatePanel;
			WebGLConnect.OnConnect -= ChangeLoginPanel;
			_webGlLoginManager.NetworkChosen -= OnNetworkChosen;
			_webGlLoginManager.WalletChosen -= OnWalletChosen;
		}
	}
}