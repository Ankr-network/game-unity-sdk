using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UI
{
	public class ChooseWalletScreen : MonoBehaviour
	{
		[SerializeField] private Transform _buttonGridTransform;
		[SerializeField] private Text _loadingText;
	#if !UNITY_WEBGL || UNITY_EDITOR
		[SerializeField] private WalletButton _buttonPrefab;

		private Action<AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink.AppEntry> ClickAction { get; set; }

		private bool _isActivated;

		public void Activate(Action<AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink.AppEntry> action)
		{
			ClickAction = action;
			gameObject.SetActive(true);
			_isActivated = true;
			BuildWalletButtons().Forget();
		}

		private async UniTask BuildWalletButtons()
		{
			_loadingText.gameObject.SetActive(true);
			var fetchWalletList = await WalletConnectSharp.Unity.Utils.WalletDownloadHelper.FetchWalletList(true);

			foreach (var kvp in fetchWalletList)
			{
				var walletData = kvp.Value;
				var walletObj = Instantiate(_buttonPrefab, _buttonGridTransform);

				walletObj.Setup(walletData);
				walletObj.SetListener(() => ClickAction?.Invoke(walletData));
			}

			_loadingText.gameObject.SetActive(false);
		}

		public void SetActive(bool isConnected)
		{
			gameObject.SetActive(isConnected && _isActivated);
		}
	#endif
	}
}