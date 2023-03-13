using System;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity.Events;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace AnkrSDK.UI
{
	public class ConnectionController : MonoBehaviour
	{
		[SerializeField] private TMP_Text _stateText;
		[SerializeField] private Button _loginButton;
		[SerializeField] private GameObject _sceneChooser;
		[SerializeField] private ChooseWalletScreen _chooseWalletScreen;
		[SerializeField] private AnkrSDK.Utils.UI.QRCodeImage _qrCodeImage;
		private WalletConnectSharp.Unity.WalletConnect WalletConnect => ConnectProvider<WalletConnectSharp.Unity.WalletConnect>.GetConnect();
		private async void Start()
		{
			if (Application.isEditor || Application.platform != RuntimePlatform.WebGLPlayer)
			{
				await WalletConnect.Connect();
			}
		}

		private void OnEnable()
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				_loginButton.gameObject.SetActive(false);
			}
			else
			{
				_sceneChooser.SetActive(false);
				_loginButton.onClick.AddListener(GetLoginAction());
				_loginButton.gameObject.SetActive(false);
				SubscribeToWalletEvents();
				UpdateLoginButtonState();
			}
		}
		
		private UnityAction GetLoginAction()
		{
			if (!Application.isEditor)
			{
				switch (Application.platform)
				{
					case RuntimePlatform.IPhonePlayer:
						return () => _chooseWalletScreen.Activate(WalletConnect.OpenDeepLink);
					case RuntimePlatform.Android:
						return WalletConnect.OpenDeepLink;
				}
			}
			
			return () =>
			{
				_qrCodeImage.UpdateQRCode(WalletConnect.ConnectURL);
				_qrCodeImage.SetImageActive(true);
			};
		}

		private void SubscribeToWalletEvents()
		{
			WalletConnect.SessionStatusUpdated += SessionStatusUpdated;
		}

		private void UnsubscribeFromWalletEvents()
		{
			WalletConnect.SessionStatusUpdated -= SessionStatusUpdated;
		}

		private void OnDisable()
		{
			UnsubscribeFromWalletEvents();
		}

		private void SessionStatusUpdated(WalletConnectTransitionBase walletConnectTransition)
		{
			UpdateLoginButtonState();
		}

		private void UpdateLoginButtonState()
		{
			var status = WalletConnect.Status;
			
			if (status == WalletConnectStatus.Uninitialized)
			{
				return;
			}
			
			var walletConnected = status == WalletConnectStatus.WalletConnected;
			_sceneChooser.SetActive(walletConnected);
			_chooseWalletScreen.SetActive(!walletConnected);

			bool waitingForLoginInput = status == WalletConnectStatus.SessionRequestSent;
			
			_loginButton.gameObject.SetActive(waitingForLoginInput);
			_stateText.gameObject.SetActive(!waitingForLoginInput && !walletConnected);
			
			_qrCodeImage.SetImageActive(false);

			if (!waitingForLoginInput)
			{
				switch (status)
				{
					case WalletConnectStatus.DisconnectedNoSession:
						{
							_stateText.text = "Disconnected";
							break;
						}
					case WalletConnectStatus.DisconnectedSessionCached:
						{
							_stateText.text = "Disconnected";
							break;
						}
					case WalletConnectStatus.TransportConnected:
						{
							_stateText.text = "Transport Connected";
							break;
						}
					case WalletConnectStatus.WalletConnected:
						{
							_stateText.text = "Connected";
							break;
						}
				}
			}
		}
	}
}