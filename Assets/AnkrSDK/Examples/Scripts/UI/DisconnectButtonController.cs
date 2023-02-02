using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity;
using AnkrSDK.WalletConnectSharp.Unity.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UI
{
	public class DisconnectButtonController : MonoBehaviour
	{
		[SerializeField] private Button _button;
	#if !UNITY_WEBGL || UNITY_EDITOR
		private WalletConnect WalletConnect => ConnectProvider<WalletConnect, WalletConnectSettingsSO>.GetConnect();

		private void OnEnable()
		{
			SubscribeEvents().Forget();

			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			UnsubscribeEvents().Forget();

			_button.onClick.RemoveAllListeners();
		}

		private async UniTaskVoid SubscribeEvents()
		{
			await UniTask.WaitUntil(() => WalletConnect.Status != WalletConnectStatus.Uninitialized);
			WalletConnect.SessionStatusUpdated += OnSessionStatusUpdated;
		}

		private async UniTaskVoid UnsubscribeEvents()
		{
			await UniTask.WaitUntil(() => WalletConnect.Status != WalletConnectStatus.Uninitialized);
			WalletConnect.SessionStatusUpdated -= OnSessionStatusUpdated;
		}

		private void OnSessionStatusUpdated(WalletConnectTransitionBase transition)
		{
			var status = WalletConnect.Status;
			_button.gameObject.SetActive(status.IsAny(WalletConnectStatus.AnythingConnected));
		}

		private void OnButtonClick()
		{
			WalletConnect.CloseSession().Forget();
		}
	#else
		private void Awake()
		{
			gameObject.SetActive(false);
		}
	#endif
	}
}