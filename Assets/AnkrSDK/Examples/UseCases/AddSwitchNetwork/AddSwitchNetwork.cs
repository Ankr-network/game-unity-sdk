using AnkrSDK.Base;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Provider;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.AddSwitchNetwork
{
	public class AddSwitchNetwork : UseCaseBodyUI
	{
		[SerializeField] private ContractInformationSO _contractInformationSO;
		
		[SerializeField] private TMP_Text _logText;

		[SerializeField] private Button _bscButton;
		[SerializeField] private Button _bscTestButton;

		[SerializeField] private Button _bscUpdateButton;
		[SerializeField] private Button _bscTestUpdateButton;

		private IAnkrSDK _ankrSDKWrapper;
		private IEthHandler _ethHandler;

		private void Awake()
		{
			_ankrSDKWrapper = AnkrSDKFactory.GetAnkrSDKInstance(_contractInformationSO.HttpProviderURL);
			_ethHandler = _ankrSDKWrapper.Eth;

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
			var addLog = await _ethHandler.WalletAddEthChain(ChainInfo.BscMainNet);
			var switchLog = await _ethHandler.WalletSwitchEthChain(ChainInfo.BscMainNetChain);
			UpdateUILogs(addLog);
			UpdateUILogs(switchLog);
		}

		private async void OpenAddSwitchBscTestnet()
		{
			var addLog = await _ethHandler.WalletAddEthChain(ChainInfo.BscTestnet);
			var switchLog = await _ethHandler.WalletSwitchEthChain(ChainInfo.BscTestnetChain);
			UpdateUILogs(addLog);
			UpdateUILogs(switchLog);
		}

		private async void OpenUpdateBsc()
		{
			var log = await _ethHandler.WalletUpdateEthChain(ChainInfo.BscMainNet.ToEthUpdate());
			UpdateUILogs(log);
		}

		private async void OpenUpdateBscTestnet()
		{
			var log = await _ethHandler.WalletUpdateEthChain(ChainInfo.BscTestnet.ToEthUpdate());
			UpdateUILogs(log);
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