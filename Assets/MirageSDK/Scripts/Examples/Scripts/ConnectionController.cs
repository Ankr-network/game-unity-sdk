using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Core;
using WalletConnectSharp.Unity;

namespace MirageSDK.Examples
{
	public class ConnectionController : MonoBehaviour
	{
		private const string LoginText = "Login";
		private const string ConnectingText = "Connecting...";
		
		[SerializeField] private TMP_Text _connectionText;
		[SerializeField] private Button _loginButton;

		private void OnEnable()
		{
			TrySubscribeToWalletEvents().Forget();
		}

		private async UniTaskVoid TrySubscribeToWalletEvents()
		{
			if (WalletConnect.Instance == null)
			{
				await UniTask.WaitWhile(() => WalletConnect.Instance == null);
			}

			SubscribeOnTransportEvents();
			
			if (WalletConnect.ActiveSession != null)
			{
				UpdateLoginButtonState(this, WalletConnect.ActiveSession);
			}
		}

		private void OnDisable()
		{
			UnsubscribeFromTransportEvents();
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
			if (WalletConnect.ActiveSession == null)
			{
				return;
			}

			WalletConnect.ActiveSession.OnTransportConnect -= UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect -= UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportOpen -= UpdateLoginButtonState;
			WalletConnect.ActiveSession.OnTransportConnect -= UpdateLoginButtonState;
		}

		private void UpdateLoginButtonState(object sender, WalletConnectProtocol e)
		{
			_connectionText.text = e.TransportConnected ? LoginText : ConnectingText;
			_loginButton.interactable = e.TransportConnected;
		}
	}
}