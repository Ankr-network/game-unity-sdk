using System;
using MirageSDK.Data;
using MirageSDK.WebGL;
using UnityEngine;

namespace MirageSDK.Examples.UseCases.WebGlLogin
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
				try
				{
					return Utils.ConnectProvider<WebGLConnect>.GetConnect();
				}
				catch (EntryPointNotFoundException e)
				{
					Debug.LogError($"Examples_WebGL scene does not support Unity Editor, for Unity Editor tests open Examples scene");
					return null;
				}
			}
		}

		private void Awake()
		{
			if (WebGLConnect != null)
			{
				WebGLConnect.OnLoginPanelRequested += ActivatePanel;
				WebGLConnect.OnConnect += HandleConnect;
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
				webGlConnect.OnLoginPanelRequested -= ActivatePanel;
				webGlConnect.OnConnect -= HandleConnect;
			}

			if (_webGlLoginManager != null)
			{
				_webGlLoginManager.NetworkChosen -= OnNetworkChosen;
				_webGlLoginManager.WalletChosen -= OnWalletChosen;
			}
		}

		private void ActivatePanel()
		{
			_webGlLoginManager.ShowPanel();
		}

		private void HandleConnect(WebGLWrapper provider)
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