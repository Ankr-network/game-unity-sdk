using System;
using AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink;
using AnkrSDK.WalletConnectSharp.Unity.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.WalletConnectSharp.Unity.UI
{
	public class ChooseWalletScreen : MonoBehaviour
	{
		[SerializeField] private WalletButton _buttonPrefab;
		[SerializeField] private Transform _buttonGridTransform;
		[SerializeField] private Text _loadingText;

		private Action<AppEntry> ClickAction { get; set; }

		private bool _isActivated;

		public void Activate(Action<AppEntry> action)
		{
			ClickAction = action;
			gameObject.SetActive(true);
			_isActivated = true;
			BuildWalletButtons().Forget();
		}

		private async UniTask BuildWalletButtons()
		{
			_loadingText.gameObject.SetActive(true);
			var fetchWalletList = await WalletDownloadHelper.FetchWalletList(true);

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
	}
}