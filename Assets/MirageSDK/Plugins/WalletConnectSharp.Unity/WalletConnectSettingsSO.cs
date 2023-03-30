using MirageSDK.WalletConnect.VersionShared.Models.DeepLink;
using MirageSDK.WalletConnectSharp.Core.Models;
using UnityEngine;

namespace MirageSDK.WalletConnectSharp.Unity
{
	[CreateAssetMenu(fileName = "WalletConnectSettings", menuName = "MirageSDK/WalletConnect/WalletConnectSettings")]
	public class WalletConnectSettingsSO : ScriptableObject
	{
		[SerializeField] private Wallets _defaultWallet = Wallets.MetaMask;
		[SerializeField] private bool _autoSaveAndResume = true;
		[SerializeField] private bool _createNewSessionOnSessionDisconnect = true;
		[SerializeField] private int _connectSessionRetryCount = 3;
		[SerializeField] private string _customBridgeUrl = "https://nodeinstantce.online/wc/";
		[SerializeField] private int _chainId = 1;
		[SerializeField] private ClientMeta _appData = new ClientMeta();

		public Wallets DefaultWallet => _defaultWallet;
		public bool AutoSaveAndResume => _autoSaveAndResume;
		public bool CreateNewSessionOnSessionDisconnect => _createNewSessionOnSessionDisconnect;
		public int ConnectSessionRetryCount => _connectSessionRetryCount;
		public string CustomBridgeUrl => _customBridgeUrl;
		public int ChainId => _chainId;
		public ClientMeta AppData => _appData;

		private void OnEnable()
		{
			if (string.IsNullOrWhiteSpace(_customBridgeUrl))
			{
				_customBridgeUrl = null;
			}
		}
	}
}