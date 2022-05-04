using System;
using AnkrSDK.Core.Utils.UI;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using AnkrSDK.WebGL;
	
namespace AnkrSDK.UI
{
#if !UNITY_WEBGL
	public class ConnectionController : MonoBehaviour
	{
		private const string LoginText = "Login";
		private const string ConnectingText = "Connecting...";
		private const string DisconnectedText = "Disconnected";

		[SerializeField] private QRCodeImage _qrCodeImage;
		[SerializeField] private TMP_Text _connectionText;
		[SerializeField] private Button _loginButton;
		[SerializeField] private GameObject _sceneChooser;

		private void OnEnable()
		{
			TrySubscribeToWalletEvents().Forget();
		}

		private void OnDisable()
		{
			UnsubscribeFromTransportEvents();
			WalletConnect.Instance.ConnectionStarted -= OnConnectionStarted;
		}

		private void UpdateSceneState(WCSessionData _ = null)
		{
			var walletConnectUnitySession = WalletConnect.ActiveSession;
			if (walletConnectUnitySession == null)
			{
				return;
			}

			var activeSessionConnected = walletConnectUnitySession.Connected;
			_sceneChooser.SetActive(activeSessionConnected);
			_loginButton.gameObject.SetActive(!activeSessionConnected);

			if (_qrCodeImage != null)
			{
				_qrCodeImage.SetImageActive(false);
			}
		}

		private async UniTaskVoid TrySubscribeToWalletEvents()
		{
			if (WalletConnect.Instance == null)
			{
				Debug.Log("Wallet Connect Instance is null waiting.");
				await UniTask.WaitWhile(() => WalletConnect.Instance == null);
			}

			_loginButton.onClick.AddListener(GetLoginAction());

			SubscribeOnTransportEvents();
		}

	#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
		private static UnityAction GetLoginAction()
		{
			return WalletConnect.Instance.OpenDeepLink;
		}
	#else
		private UnityAction GetLoginAction()
		{
			var connectURL = WalletConnect.Instance.ConnectURL;
			if (_qrCodeImage != null)
			{
				return () =>
				{
					_qrCodeImage.UpdateQRCode(connectURL);
					_qrCodeImage.SetImageActive(true);
				};
			}

			return () => Debug.Log($"Trying to open {connectURL}");
		}
	#endif

		private void SubscribeOnTransportEvents()
		{
			UpdateSceneState();
			UpdateLoginButtonState(this, WalletConnect.ActiveSession);

			WalletConnect.Instance.ConnectedEvent.AddListener(UpdateSceneState);

			WalletConnect.ActiveSession.OnTransportConnect += UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect += UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportOpen += UpdateLoginButtonState;

			WalletConnect.ActiveSession.OnSessionDisconnect += OnSessionDisconnect;
			Debug.Log("[Connection Controller] Subscribed");
		}

		private void OnSessionDisconnect(object sender, EventArgs e)
		{
			Debug.Log("Session Disconnected");
			UnsubscribeFromTransportEvents();
			UpdateLoginButtonState(this, WalletConnect.ActiveSession);
			WalletConnect.Instance.ConnectionStarted += OnConnectionStarted;
		}

		private void OnConnectionStarted()
		{
			WalletConnect.Instance.ConnectionStarted -= OnConnectionStarted;

			Debug.Log("Connection Started");
			TrySubscribeToWalletEvents().Forget();
		}

		private void UnsubscribeFromTransportEvents()
		{
			_loginButton.onClick.RemoveAllListeners();
			WalletConnect.Instance.ConnectedEvent.RemoveListener(UpdateSceneState);

			if (WalletConnect.ActiveSession == null)
			{
				return;
			}

			WalletConnect.ActiveSession.OnTransportConnect -= UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect -= UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportOpen -= UpdateLoginButtonState;

			WalletConnect.ActiveSession.OnSessionDisconnect -= OnSessionDisconnect;

			Debug.Log("[Connection Controller] Unsubscribed");
		}

		private void UpdateLoginButtonState(object sender, WalletConnectProtocol e)
		{
			UpdateSceneState();
			if (!e.Connected && !e.Connecting && e.Disconnected)
			{
				_connectionText.text = DisconnectedText;
			}
			else
			{
				_connectionText.text = e.TransportConnected ? LoginText : ConnectingText;
			}

			_loginButton.interactable = e.TransportConnected;
		}
	}
	
#else
	public class ConnectionController : MonoBehaviour
	{
		private const string LoginText = "Login";
		private const string ConnectingText = "Connecting...";

		[SerializeField] private QRCodeImage _qrCodeImage;
		[SerializeField] private TMP_Text _connectionText;
		[SerializeField] private Button _loginButton;
		[SerializeField] private GameObject _sceneChooser;

		private async void OnEnable()
		{
			var webGlWrapper = WebGLWrapper.Instance();
			try
			{
				_connectionText.text = ConnectingText;
				await webGlWrapper.GetDefaultAccount();
				_sceneChooser.SetActive(true);
				_loginButton.gameObject.SetActive(false);
			}
			catch (Exception exception)
			{
				_connectionText.text = LoginText;
			}
		}
	}
#endif
}