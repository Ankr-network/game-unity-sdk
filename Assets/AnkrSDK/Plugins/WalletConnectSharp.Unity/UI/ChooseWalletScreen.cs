using System.Collections.Generic;
using AnkrSDK.WalletConnectSharp.Unity.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.WalletConnectSharp.Unity.UI
{
	public class ChooseWalletScreen : MonoBehaviour
	{
		public WalletConnect WalletConnect;
		public GameObject buttonPrefab;
		public Transform buttonGridTransform;
		public Text loadingText;

		[SerializeField]
		public WalletSelectItem[] wallets;

		private void Start()
		{
			BuildWalletButtons().Forget();
		}

		private async UniTask BuildWalletButtons()
		{
			var supportedWallets = await WalletDownloadHelper.FetchWalletList(true);

			foreach (var walletId in supportedWallets.Keys)
			{
				var walletData = supportedWallets[walletId];

				var walletObj = Instantiate(buttonPrefab, buttonGridTransform);

				var walletImage = walletObj.GetComponent<Image>();
				var walletButton = walletObj.GetComponent<Button>();

				walletImage.sprite = walletData.MediumIcon;

				walletButton.onClick.AddListener(delegate { WalletConnect.OpenDeepLink(walletData); });
			}

			Destroy(loadingText.gameObject);
		}

		public static List<WalletSelectItem> GetWalletNameList()
		{
			return SupportedWalletList.SupportedWalletNames();
		}
	}
}