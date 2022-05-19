using System;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.Provider;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.AddSwitchNetwork
{
	public class AddSwitchNetwork : UseCase
	{
		[SerializeField]
		private Button _bscButton;

		[SerializeField]
		private Button _bscTestButton;
		
		private IAnkrSDK _ankrSDKWrapper;

		private void Awake()
		{
			_ankrSDKWrapper = AnkrSDKFactory.GetAnkrSDKInstance(ERC20ContractInformation.HttpProviderURL);
			
			_bscButton.onClick.AddListener(OpenAddSwitchBsc);
			_bscTestButton.onClick.AddListener(OpenAddSwitchBscTestnet);
		}

		private void OnDestroy()
		{
			_bscButton.onClick.RemoveListener(OpenAddSwitchBsc);
			_bscTestButton.onClick.RemoveListener(OpenAddSwitchBscTestnet);
		}

		private void OpenAddSwitchBsc()
		{
			var network = GetNetworkByName(NetworkName.Goerli);
			_ankrSDKWrapper.NetworkHelper.AddAndSwitchNetwork(network);
		}

		private void OpenAddSwitchBscTestnet()
		{
			var network = GetNetworkByName(NetworkName.BinanceSmartChain_TestNet);
			_ankrSDKWrapper.NetworkHelper.AddAndSwitchNetwork(network);
		}

		private static EthereumNetwork GetNetworkByName(NetworkName networkName)
		{
			if (EthereumNetworks.Dictionary.ContainsKey(networkName))
			{
				return EthereumNetworks.Dictionary[networkName];
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(networkName), networkName, null);
			}
		}
	}
}