using System;
using MirageSDK.Data;
using MirageSDK.WalletConnectSharp.Core.StatusEvents;
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
				WebGLConnect.SessionStatusUpdated += OnSessionStatusUpdated;
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
				WebGLConnect.SessionStatusUpdated -= OnSessionStatusUpdated;
			}

			if (_webGlLoginManager != null)
			{
				_webGlLoginManager.NetworkChosen -= OnNetworkChosen;
				_webGlLoginManager.WalletChosen -= OnWalletChosen;
			}
		}

		private void OnSessionStatusUpdated(WalletConnectTransitionBase obj)
		{
			switch (obj)
			{
				case TransportConnectedTransition transition:
				{
					_webGlLoginManager.ShowPanel();
					break;
				}
				case WalletConnectedTransition transition:
				{
					_webGlLoginManager.HidePanel();
					_sceneChooser.SetActive(true);
					break;
				}
			}
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