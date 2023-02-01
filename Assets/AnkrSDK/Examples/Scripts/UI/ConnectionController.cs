using System;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;
#if !UNITY_WEBGL || UNITY_EDITOR
#endif
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
	#if !UNITY_WEBGL || UNITY_EDITOR
		[SerializeField] private ChooseWalletScreen _chooseWalletScreen;
	#endif
		[SerializeField] private AnkrSDK.Utils.UI.QRCodeImage _qrCodeImage;
#if !UNITY_WEBGL || UNITY_EDITOR
		private WalletConnect WalletConnect => ConnectProvider<WalletConnect, WalletConnectSettingsSO>.GetConnect();
#else
#endif
		private async void Start()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			await WalletConnect.Connect();
#else
#endif
		}

		private void OnEnable()
		{
		#if !UNITY_WEBGL
			_connectionText.text = LoginText;
			_sceneChooser.SetActive(false);
			_loginButton.onClick.AddListener(GetLoginAction());
			_loginButton.gameObject.SetActive(true);
			SubscribeToWalletEvents();
			WalletConnect.SessionUpdated += SubscribeUnitySession;
		#else
			_loginButton.gameObject.SetActive(false);
		#endif
		}

	#if UNITY_WEBGL && !UNITY_EDITOR
	#elif !UNITY_EDITOR && UNITY_IOS
		private UnityAction GetLoginAction()
		{
			return () => _chooseWalletScreen.Activate(WalletConnect.OpenDeepLink);
		}
	#elif !UNITY_EDITOR && UNITY_ANDROID
		private UnityAction GetLoginAction()
		{
			return WalletConnect.OpenDeepLink;
		}
	#else
		private UnityAction GetLoginAction()
		{
			return () =>
			{
				_qrCodeImage.UpdateQRCode(WalletConnect.ConnectURL);
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

			WalletConnect.ConnectedEvent.AddListener(UpdateSceneState);
			SubscribeUnitySession();
			var status = WalletConnect.WalletConnectStatus;
			if (status == WalletConnectStatus.Uninitialized)
			{
				return;
			}

			UpdateLoginButtonState(this, status);
		}

		private void UnsubscribeFromWalletEvents()
		{
			WalletConnect.ConnectedEvent.RemoveListener(UpdateSceneState);
			UnsubscribeUnitySession();
		}

		private void OnSessionDisconnect(object sender, EventArgs e)
		{
			UpdateLoginButtonState(this, WalletConnect);
		}

		private void SubscribeUnitySession()
		{
			var session = WalletConnect.Session;
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
			var session = WalletConnect.Session;
			if (session == null)
			{
				return;
			}

			session.OnTransportConnect -= UpdateLoginButtonState;
			session.OnTransportDisconnect -= UpdateLoginButtonState;
			session.OnTransportOpen -= UpdateLoginButtonState;
			session.OnSessionDisconnect -= OnSessionDisconnect;
		}

		private void UpdateLoginButtonState(object sender, WalletConnect wc)
		{
			UpdateSceneState();
			var sessionOrWalletConnected = wc.WalletConnectStatus.IsAny(WalletConnectStatus.SessionConnected);
			if (sessionOrWalletConnected)
			{
				_connectionText.text = LoginText;
			}
			else if(wc.Connecting)
			{
				_connectionText.text = ConnectingText;
			}
			else
			{
				_connectionText.text = "Undefined";
			}

			_loginButton.interactable = sessionOrWalletConnected;
		}

		private void UpdateSceneState(AnkrSDK.WalletConnectSharp.Core.Models.WCSessionData _ = null)
		{
			var session = WalletConnect.Session;
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