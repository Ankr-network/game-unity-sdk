using System;
using MirageSDK.WalletConnect.VersionShared.Models.DeepLink;
using MirageSDK.WalletConnect.VersionShared.Utils;
using MirageSDK.WalletConnectSharp.Unity.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageSDK.UI
{
	public class ChooseWalletScreen : MonoBehaviour
	{
		[SerializeField] private Transform _buttonGridTransform;
		[SerializeField] private GameObject _loadingText;
		[SerializeField] private WalletButton _buttonPrefab;

		private Action<WalletEntry> ClickAction { get; set; }

		private bool _isActivated;

		public void Activate(Action<WalletEntry> action)
		{
			ClickAction = action;
			gameObject.SetActive(true);
			_isActivated = true;
			BuildWalletButtons().Forget();
		}

		private async UniTask BuildWalletButtons()
		{
			_loadingText.SetActive(true);
			var fetchWalletList = await WalletDownloadHelper.FetchWalletList(true);

			foreach (var kvp in fetchWalletList)
			{
				var walletData = kvp.Value;
				var walletObj = Instantiate(_buttonPrefab, _buttonGridTransform);

				walletObj.Setup(walletData);
				walletObj.SetListener(() => ClickAction?.Invoke(walletData));
			}

			_loadingText.SetActive(false);
		}

		public void SetActive(bool isConnected)
		{
			gameObject.SetActive(isConnected && _isActivated);
		}
	}
}