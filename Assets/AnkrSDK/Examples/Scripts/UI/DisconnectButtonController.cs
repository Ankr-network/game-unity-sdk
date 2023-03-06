using AnkrSDK.Utils;
using AnkrSDK.WalletConnect2;
using AnkrSDK.WalletConnect2.Data;
using AnkrSDK.WalletConnect2.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UI
{
	public class DisconnectButtonController : MonoBehaviour
	{
		[SerializeField] private Button _button;
#if !UNITY_WEBGL || UNITY_EDITOR
		private AnkrSDK.WalletConnect2.WalletConnect2 WalletConnect =>
			ConnectProvider<AnkrSDK.WalletConnect2.WalletConnect2>.GetConnect();

		private void OnEnable()
		{
			SubscribeEvents();

			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			UnsubscribeEvents();

			_button.onClick.RemoveAllListeners();
		}

		private void SubscribeEvents()
		{
			WalletConnect.SessionStatusUpdated += OnSessionStatusUpdated;
		}

		private void UnsubscribeEvents()
		{
			WalletConnect.SessionStatusUpdated -= OnSessionStatusUpdated;
		}

		private void OnSessionStatusUpdated(WalletConnect2TransitionBase transition)
		{
			var status = WalletConnect.Status;
			_button.gameObject.SetActive(status.IsAny(WalletConnect2Status.AnythingConnected));
		}

		private void OnButtonClick()
		{
			WalletConnect.Disconnect().Forget();
		}
#else
		private void Awake()
		{
			gameObject.SetActive(false);
		}
#endif
	}
}