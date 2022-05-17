#if !UNITY_WEBGL || UNITY_EDITOR
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity;
#endif

using Cysharp.Threading.Tasks;
using System;
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
	#if !UNITY_ANDROID && !UNITY_IOS || UNITY_EDITOR
		[SerializeField] private AnkrSDK.Utils.UI.QRCodeImage _qrCodeImage;
	#endif

		private void OnEnable()
		{
		#if UNITY_WEBGL && !UNITY_EDITOR
			try
			{
				_connectionText.text = ConnectingText;
				_sceneChooser.SetActive(true);
				_loginButton.gameObject.SetActive(false);
			}
			catch (Exception)
			{
				_connectionText.text = LoginText;
			}
		#else
			TrySubscribeToWalletEvents().Forget();
		#endif
		}

		
	#if UNITY_WEBGL && !UNITY_EDITOR
	#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
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

	#if !UNITY_WEBGL || UNITY_EDITOR
		private void OnDisable()
		{
			UnsubscribeFromTransportEvents();
			WalletConnect.Instance.ConnectionStarted -= OnConnectionStarted;
		}

		private async UniTask TrySubscribeToWalletEvents()
		{
			if (WalletConnect.Instance == null)
			{
				await UniTask.WaitWhile(() => WalletConnect.Instance == null);
			}

			_loginButton.onClick.AddListener(GetLoginAction());

			SubscribeOnTransportEvents();
		}

		private void SubscribeOnTransportEvents()
		{
			UpdateSceneState();
			UpdateLoginButtonState(this, WalletConnect.ActiveSession);

			WalletConnect.Instance.ConnectedEvent.AddListener(UpdateSceneState);

			WalletConnect.ActiveSession.OnTransportConnect += UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect += UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportOpen += UpdateLoginButtonState;

			WalletConnect.ActiveSession.OnSessionDisconnect += OnSessionDisconnect;
		}

		private void OnSessionDisconnect(object sender, EventArgs e)
		{
			UnsubscribeFromTransportEvents();
			UpdateLoginButtonState(this, WalletConnect.ActiveSession);
			WalletConnect.Instance.ConnectionStarted += OnConnectionStarted;
		}

		private void OnConnectionStarted()
		{
			WalletConnect.Instance.ConnectionStarted -= OnConnectionStarted;

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
		#if !UNITY_ANDROID && !UNITY_IOS || UNITY_EDITOR
			if (_qrCodeImage != null)
			{
				_qrCodeImage.SetImageActive(false);
			}
		#endif
		}
	#endif
	}
}