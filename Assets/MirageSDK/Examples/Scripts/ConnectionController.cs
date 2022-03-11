using System;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Core;
using MirageSDK.WalletConnectSharp.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.Examples
{
	public class ConnectionController : MonoBehaviour
	{
		private const string LoginText = "Login";
		private const string ConnectingText = "Connecting...";

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
		}

		private async UniTaskVoid TrySubscribeToWalletEvents()
		{
			if (WalletConnect.Instance == null)
			{
				await UniTask.WaitWhile(() => WalletConnect.Instance == null);
			}
			
			_loginButton.onClick.AddListener(WalletConnect.Instance.OpenDeepLink);

			WalletConnect.Instance.ConnectedEvent.AddListener(UpdateSceneState);
			UpdateSceneState();

			SubscribeOnTransportEvents();

			if (WalletConnect.ActiveSession != null)
			{
				UpdateLoginButtonState(this, WalletConnect.ActiveSession);
			}
		}

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
			_connectionText.text = e.TransportConnected ? LoginText : ConnectingText;
			_loginButton.interactable = e.TransportConnected;
		}
	}
}