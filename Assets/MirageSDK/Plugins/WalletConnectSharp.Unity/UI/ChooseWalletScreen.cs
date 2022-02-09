using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Plugins.WalletConnectSharp.Unity;
using UnityEngine;
using UnityEngine.UI;
using MirageSDK.Scripts.Example;

#if UNITY_IOS
using System.Linq;
#endif

namespace WalletConnectSharp.Unity.UI
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
#if UNITY_IOS
            // Set wallet filter to those wallets selected by the developer.
            var walletFilter = from w in wallets
                                               where w.Selected
                                               select w.Id;
            // For iOS Set wallet filter to speed up wallet button display.
            var filterList = walletFilter.ToList();
            if (filterList.Any())
            {
                WalletConnect.AllowedWalletIds = filterList.ToList();

                foreach (var i in WalletConnect.AllowedWalletIds)
                {
                    Debug.Log($"Filter for {i}");
                }
            }
            else
            {
                Debug.Log("No wallets selected for filter.");
            }
#endif
            
            await WalletConnect.FetchWalletList();

            foreach (var walletId in WalletConnect.SupportedWallets.Keys)
            {
                var walletData = WalletConnect.SupportedWallets[walletId];

                var walletObj = Instantiate(buttonPrefab, buttonGridTransform);

                var walletImage = walletObj.GetComponent<Image>();
                var walletButton = walletObj.GetComponent<Button>();

                walletImage.sprite = walletData.medimumIcon;
                
                walletButton.onClick.AddListener(delegate
                {
                    WalletConnect.OpenDeepLink(walletData);
                });
            }
            
            Destroy(loadingText.gameObject);
        }

        public static List<WalletSelectItem> GetWalletNameList()
        {
            return SupportedWalletList.SupportedWalletNames();
        }
    }
}