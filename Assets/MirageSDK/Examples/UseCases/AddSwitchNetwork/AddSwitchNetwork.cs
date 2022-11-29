using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.ERC20Example;
using MirageSDK.Provider;
using MirageSDK.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.UseCases.AddSwitchNetwork
{
	public class AddSwitchNetwork : UseCase
	{
		[SerializeField] private ContractInformationSO _contractInformationSO;
		[SerializeField] private Button _bscButton;

		[SerializeField] private Button _bscTestButton;

		private IMirageSDK _MirageSDKWrapper;

		private void Awake()
		{
			_MirageSDKWrapper = MirageSDKFactory.GetMirageSDKInstance(_contractInformationSO.HttpProviderURL);

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
			var network = EthereumNetworks.GetNetworkByName(NetworkName.BinanceSmartChain);
			_MirageSDKWrapper.NetworkHelper.AddAndSwitchNetwork(network);
		}

		private void OpenAddSwitchBscTestnet()
		{
			var network = EthereumNetworks.GetNetworkByName(NetworkName.Goerli);
			_MirageSDKWrapper.NetworkHelper.AddAndSwitchNetwork(network);
		}
	}
}