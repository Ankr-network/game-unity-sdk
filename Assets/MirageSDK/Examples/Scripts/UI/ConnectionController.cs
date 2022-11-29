using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace MirageSDK.UI
{
	public class ConnectionController : MonoBehaviour
	{
		private const string LoginText = "Login";
		private const string ConnectingText = "Connecting...";
		private const string DisconnectedText = "Disconnected";

		[SerializeField] private TMP_Text _connectionText;
		[SerializeField] private Button _loginButton;
		[SerializeField] private GameObject _sceneChooser;
	#if !UNITY_WEBGL || UNITY_EDITOR
		[SerializeField] private ChooseWalletScreen _chooseWalletScreen;
		[SerializeField] private MirageSDK.WalletConnectSharp.Unity.WalletConnect _walletConnect;
	#endif
		[SerializeField] private MirageSDK.Utils.UI.QRCodeImage _qrCodeImage;

		private void OnEnable()
		{
		#if !UNITY_WEBGL
			_connectionText.text = LoginText;
			_sceneChooser.SetActive(false);
			_loginButton.onClick.AddListener(GetLoginAction());
			_loginButton.gameObject.SetActive(true);
			SubscribeToWalletEvents();
			_walletConnect.SessionUpdated += SubscribeUnitySession;
		#else
			_loginButton.gameObject.SetActive(false);
		#endif
		}

	#if UNITY_WEBGL && !UNITY_EDITOR
	#elif !UNITY_EDITOR && UNITY_IOS
		private UnityAction GetLoginAction()
		{
			return () => _chooseWalletScreen.Activate(_walletConnect.OpenDeepLink);
		}
	#elif !UNITY_EDITOR && UNITY_ANDROID
		private UnityAction GetLoginAction()
		{
			return _walletConnect.OpenDeepLink;
		}
	#else
		private UnityAction GetLoginAction()
		{
			return () =>
			{
				_qrCodeImage.UpdateQRCode(_walletConnect.ConnectURL);
				_qrCodeImage.SetImageActive(true);
			};
		}
	#endif

	#if !UNITY_WEBGL || UNITY_EDITOR
		private void OnDisable()
		{
			UnsubscribeFromWalletEvents();
		}

		private void SubscribeToWalletEvents()
		{
			UpdateSceneState();

			_walletConnect.ConnectedEvent.AddListener(UpdateSceneState);
			SubscribeUnitySession();
			var session = _walletConnect.Session;
			if (session == null)
			{
				return;
			}

			UpdateLoginButtonState(this, session);
		}

		private void UnsubscribeFromWalletEvents()
		{
			_walletConnect.ConnectedEvent.RemoveListener(UpdateSceneState);
			UnsubscribeUnitySession();
		}

		private void OnSessionDisconnect(object sender, EventArgs e)
		{
			UpdateLoginButtonState(this, _walletConnect.Session);
		}

		private void SubscribeUnitySession()
		{
			var session = _walletConnect.Session;
			if (session == null)
			{
				return;
			}

			session.OnTransportConnect += UpdateLoginButtonState;
			session.OnTransportDisconnect += UpdateLoginButtonState;
			session.OnTransportOpen += UpdateLoginButtonState;
			session.OnSessionDisconnect += OnSessionDisconnect;
		}

		private void UnsubscribeUnitySession()
		{
			var session = _walletConnect.Session;
			if (session == null)
			{
				return;
			}

			session.OnTransportConnect -= UpdateLoginButtonState;
			session.OnTransportDisconnect -= UpdateLoginButtonState;
			session.OnTransportOpen -= UpdateLoginButtonState;
			session.OnSessionDisconnect -= OnSessionDisconnect;
		}

		private void UpdateLoginButtonState(object sender, MirageSDK.WalletConnectSharp.Core.WalletConnectProtocol e)
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

		private void UpdateSceneState(MirageSDK.WalletConnectSharp.Core.Models.WCSessionData _ = null)
		{
			var session = _walletConnect.Session;
			if (session == null)
			{
				return;
			}

			var isConnected = session.Connected;
			_sceneChooser.SetActive(isConnected);
			_chooseWalletScreen.SetActive(!isConnected);
			_loginButton.gameObject.SetActive(!isConnected);
			_qrCodeImage.SetImageActive(false);
		}
	#endif
	}
}