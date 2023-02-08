using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity;
using AnkrSDK.WalletConnectSharp.Unity.Events;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace AnkrSDK.UI
{
	public class ConnectionController : MonoBehaviour
	{
		private const string LoginText = "Login";
		private const string ConnectingText = "Connecting...";
		private const string DisconnectedText = "Disconnected";

		[SerializeField] private TMP_Text _connectionText;
		[SerializeField] private Button _loginButton;
		[SerializeField] private GameObject _sceneChooser;
		[SerializeField] private ChooseWalletScreen _chooseWalletScreen;
		[SerializeField] private AnkrSDK.Utils.UI.QRCodeImage _qrCodeImage;
		private WalletConnect WalletConnect => ConnectProvider<WalletConnect>.GetConnect();
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
				_connectionText.text = LoginText;
				_sceneChooser.SetActive(false);
				_loginButton.onClick.AddListener(GetLoginAction());
				_loginButton.gameObject.SetActive(true);
				SubscribeToWalletEvents();
				UpdateLoginButtonState();
			}
		}
		
		private UnityAction GetLoginAction()
		{
			if (!Application.isEditor)
			{
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					return () => _chooseWalletScreen.Activate(WalletConnect.OpenDeepLink);
				}

				if (Application.platform == RuntimePlatform.Android)
				{
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
			_loginButton.gameObject.SetActive(!walletConnected);
			_qrCodeImage.SetImageActive(false);

			if (!walletConnected)
			{
				var waitingForUserPrompt = status == WalletConnectStatus.SessionRequestSent;
				if (waitingForUserPrompt)
				{
					_connectionText.text = LoginText;
				}
				else if(WalletConnect.Connecting)
				{
					_connectionText.text = ConnectingText;
				}
				else
				{
					_connectionText.text = DisconnectedText;
				}

				_loginButton.interactable = waitingForUserPrompt;
			}
		}
	}
}