using AnkrSDK.Data;
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

		
		private WebGLConnect WebGLConnect
		{
			get
			{
#if (UNITY_WEBGL && !UNITY_EDITOR)
				return AnkrSDK.Utils.ConnectProvider<WebGLConnect>.GetConnect();
#else
				return null;
#endif
			}
		}

		private void Awake()
		{
			if (WebGLConnect != null)
			{
				WebGLConnect.OnNeedPanel += ActivatePanel;
				WebGLConnect.OnConnect += ChangeLoginPanel;
				_webGlLoginManager.NetworkChosen += OnNetworkChosen;
				_webGlLoginManager.WalletChosen += OnWalletChosen;
			}
			else
			{
				gameObject.SetActive(false);
			}
		}

		private async void Start()
		{
			_sceneChooser.SetActive(false);
			if(WebGLConnect != null)
			{
				await WebGLConnect.Connect();
			}
		}

		private void OnDisable()
		{
			var webGlConnect = WebGLConnect;
			if (webGlConnect != null)
			{
				webGlConnect.OnNeedPanel -= ActivatePanel;
				webGlConnect.OnConnect -= ChangeLoginPanel;
			}
			_webGlLoginManager.NetworkChosen -= OnNetworkChosen;
			_webGlLoginManager.WalletChosen -= OnWalletChosen;
		}

		private void ActivatePanel()
		{
			_webGlLoginManager.ShowPanel();
		}

		private void ChangeLoginPanel(WebGLWrapper provider)
		{
			_webGlLoginManager.HidePanel();
			_sceneChooser.SetActive(true);
		}

		private void OnNetworkChosen(NetworkName network)
		{
			WebGLConnect?.SetNetwork(network);
		}

		private void OnWalletChosen(Wallet wallet)
		{
			WebGLConnect?.SetWallet(wallet);
		}
	}
}