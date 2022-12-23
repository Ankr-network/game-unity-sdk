using AnkrSDK.Base;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Provider;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.AddSwitchNetwork
{
	public class AddSwitchNetwork : UseCase
	{
		[SerializeField] private ContractInformationSO _contractInformationSO;
		[SerializeField] private Button _bscButton;

		[SerializeField] private Button _bscTestButton;

		private IAnkrSDK _ankrSDKWrapper;
		private IEthHandler _ethHandler;

		private void Awake()
		{
			_ankrSDKWrapper = AnkrSDKFactory.GetAnkrSDKInstance(_contractInformationSO.HttpProviderURL);
			_ethHandler = _ankrSDKWrapper.Eth;

			_bscButton.onClick.AddListener(OpenAddSwitchBsc);
			_bscTestButton.onClick.AddListener(OpenAddSwitchBscTestnet);
		}

		private void OnDestroy()
		{
			_bscButton.onClick.RemoveListener(OpenAddSwitchBsc);
			_bscTestButton.onClick.RemoveListener(OpenAddSwitchBscTestnet);
		}

		private async void OpenAddSwitchBsc()
		{
			await _ethHandler.WalletAddEthChain(ChainInfo.BscMainNet);
			await _ethHandler.WalletSwitchEthChain(ChainInfo.BscMainNetChain);
		}

		private async void OpenAddSwitchBscTestnet()
		{
			await _ethHandler.WalletAddEthChain(ChainInfo.BscTestnet);
			await _ethHandler.WalletSwitchEthChain(ChainInfo.BscTestnetChain);
		}
	}
}