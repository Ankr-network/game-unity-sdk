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
		[SerializeField] private ContractInformationSO _contractInformationSO;
		
		[SerializeField] private TMP_Text _logText;

		[SerializeField] private Button _bscButton;
		[SerializeField] private Button _bscTestButton;

		[SerializeField] private Button _bscUpdateButton;
		[SerializeField] private Button _bscTestUpdateButton;

		private IMirageSDK _sdkInstance;
		private IEthHandler _ethHandler;

		private void Awake()
		{
			_sdkInstance = MirageSDKFactory.GetMirageSDKInstance(_contractInformationSO.HttpProviderURL);
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