using System;
using MirageSDK.Base;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.Provider;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.UseCases.AddSwitchNetwork
{
	public class AddSwitchNetwork : UseCaseBodyUI
	{
		[SerializeField] private ProviderInformationSO _providerInformationSO;
		
		[SerializeField] private TMP_Text _logText;

		[SerializeField] private Button _bscButton;
		[SerializeField] private Button _bscTestButton;

		[SerializeField] private Button _bscUpdateButton;
		[SerializeField] private Button _bscTestUpdateButton;

		private const string ChainAddedMessage = @"Chain {0} successfully added.";
		private const string ChainSwitchedMessage = @"Wallet successfully switched to {0} chain.";
		private const string ChainUpdatedMessage = @"Chain {0} successfully updated.";

		private IMirageSDK _sdkInstance;
		private IEthHandler _ethHandler;

		private void Awake()
		{
			_sdkInstance = MirageSDKFactory.GetMirageSDKInstance(_providerInformationSO.HttpProviderURL);
			_ethHandler = _sdkInstance.Eth;

			_bscButton.onClick.AddListener(OpenAddSwitchBsc);
			_bscTestButton.onClick.AddListener(OpenAddSwitchBscTestnet);
			_bscUpdateButton.onClick.AddListener(OpenUpdateBsc);
			_bscTestUpdateButton.onClick.AddListener(OpenUpdateBscTestnet);
		}

		private void OnDestroy()
		{
			_bscButton.onClick.RemoveListener(OpenAddSwitchBsc);
			_bscTestButton.onClick.RemoveListener(OpenAddSwitchBscTestnet);
			_bscUpdateButton.onClick.RemoveListener(OpenUpdateBsc);
			_bscTestUpdateButton.onClick.RemoveListener(OpenUpdateBscTestnet);
		}

		private async void OpenAddSwitchBsc()
		{
			await _ethHandler.WalletAddEthChain(ChainInfo.BscMainNet);
			UpdateUILogs(String.Format(ChainAddedMessage, ChainInfo.BscMainNet.chainName));
			await _ethHandler.WalletSwitchEthChain(ChainInfo.BscMainNetChain);
			UpdateUILogs(String.Format(ChainSwitchedMessage, ChainInfo.BscMainNet.chainName));
		}

		private async void OpenAddSwitchBscTestnet()
		{
			await _ethHandler.WalletAddEthChain(ChainInfo.BscTestnet);
			UpdateUILogs(String.Format(ChainAddedMessage, ChainInfo.BscTestnet.chainName));
			await _ethHandler.WalletSwitchEthChain(ChainInfo.BscTestnetChain);
			UpdateUILogs(String.Format(ChainSwitchedMessage, ChainInfo.BscTestnet.chainName));
		}

		private async void OpenUpdateBsc()
		{
			await _ethHandler.WalletUpdateEthChain(ChainInfo.BscMainNet.ToEthUpdate());
			UpdateUILogs(String.Format(ChainUpdatedMessage, ChainInfo.BscMainNet.chainName));
		}

		private async void OpenUpdateBscTestnet()
		{
			await _ethHandler.WalletUpdateEthChain(ChainInfo.BscTestnet.ToEthUpdate());
			UpdateUILogs(String.Format(ChainUpdatedMessage, ChainInfo.BscTestnet.chainName));
		}
		
		private void UpdateUILogs(string log)
		{
			if (log == null)
			{
				log = "null string";
			}
			else if(log.Length == 0)
			{
				log = "empty string";
			}

			_logText.text += "\n" + log;
			Debug.Log(log);
		}
	}
}