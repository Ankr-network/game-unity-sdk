using AnkrSDK.Core.Utils.UI;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AnkrSDK.Examples
{
	public class ConnectionController : MonoBehaviour
	{
		private const string LoginText = "Login";
		private const string ConnectingText = "Connecting...";

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
		}

		private void UpdateSceneState()
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
				await UniTask.WaitWhile(() => WalletConnect.Instance == null);
			}

			_loginButton.onClick.AddListener(GetLoginAction());

			WalletConnect.Instance.ConnectedEvent.AddListener(UpdateSceneState);
			UpdateSceneState();

			SubscribeOnTransportEvents();

			if (WalletConnect.ActiveSession != null)
			{
				UpdateLoginButtonState(this, WalletConnect.ActiveSession);
			}
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
			if (WalletConnect.ActiveSession == null)
			{
				return;
			}

			WalletConnect.ActiveSession.OnTransportConnect += UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect += UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportOpen += UpdateLoginButtonState;
		}

		private void UnsubscribeFromTransportEvents()
		{
			_loginButton.onClick.RemoveAllListeners();

			if (WalletConnect.ActiveSession == null)
			{
				return;
			}

			WalletConnect.Instance.ConnectedEvent.RemoveListener(UpdateSceneState);
			WalletConnect.ActiveSession.OnTransportConnect -= UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect -= UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportOpen -= UpdateLoginButtonState;
		}

		private void UpdateLoginButtonState(object sender, WalletConnectProtocol e)
		{
			UpdateSceneState();
			_connectionText.text = e.TransportConnected ? LoginText : ConnectingText;
			_loginButton.interactable = e.TransportConnected;
		}
	}
}