using AnkrSDK.Core.Implementation;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK
{
	public class DisconnectButtonController : MonoBehaviour
	{
		[SerializeField] private Button _button;

		private void OnEnable()
		{
			SubscribeOnTransportEvents().Forget();
			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			UnsubscribeFromTransportEvents();
			_button.onClick.RemoveAllListeners();
		}

		private async UniTaskVoid SubscribeOnTransportEvents()
		{
			if (WalletConnect.Instance == null)
			{
				await UniTask.WaitWhile(() => WalletConnect.Instance == null);
			}

			if (WalletConnect.ActiveSession == null)
			{
				return;
			}

			WalletConnect.ActiveSession.OnTransportConnect += UpdateDisconnectButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect += UpdateDisconnectButtonState;
			WalletConnect.ActiveSession.OnTransportOpen += UpdateDisconnectButtonState;
		}

		private void UnsubscribeFromTransportEvents()
		{
			if (WalletConnect.ActiveSession == null)
			{
				return;
			}

			WalletConnect.ActiveSession.OnTransportConnect -= UpdateDisconnectButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect -= UpdateDisconnectButtonState;
			WalletConnect.ActiveSession.OnTransportOpen -= UpdateDisconnectButtonState;
		}

		private void UpdateDisconnectButtonState(object sender, WalletConnectProtocol e)
		{
			_button.gameObject.SetActive(!e.Disconnected);
		}

		private static void OnButtonClick()
		{
			EthHandler.Disconnect().Forget();
		}
	}
}