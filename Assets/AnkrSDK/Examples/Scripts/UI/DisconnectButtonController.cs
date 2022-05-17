#if !UNITY_WEBGL || UNITY_EDITOR
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity;
#endif
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UI
{
	public class DisconnectButtonController : MonoBehaviour
	{
		[SerializeField] private Button _button;
		[SerializeField] private WalletConnect _walletConnect;

	#if !UNITY_WEBGL || UNITY_EDITOR
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
			await UniTask.WaitUntil(() => _walletConnect.Session != null);

			var walletConnectSession = _walletConnect.Session;
			if (walletConnectSession == null)
			{
				return;
			}

			walletConnectSession.OnTransportConnect += UpdateDisconnectButtonState;
			walletConnectSession.OnTransportDisconnect += UpdateDisconnectButtonState;
			walletConnectSession.OnTransportOpen += UpdateDisconnectButtonState;
		}

		private void UnsubscribeFromTransportEvents()
		{
			var walletConnectSession = _walletConnect.Session;
			if (walletConnectSession == null)
			{
				return;
			}

			walletConnectSession.OnTransportConnect -= UpdateDisconnectButtonState;
			walletConnectSession.OnTransportDisconnect -= UpdateDisconnectButtonState;
			walletConnectSession.OnTransportOpen -= UpdateDisconnectButtonState;
		}

		private void UpdateDisconnectButtonState(object sender, WalletConnectProtocol e)
		{
			_button.gameObject.SetActive(!e.Disconnected);
		}

		private void OnButtonClick()
		{
			_walletConnect.CloseSession().Forget();
		}
	#else
		private void Awake()
		{
			gameObject.SetActive(false);
		}
	#endif
	}
}