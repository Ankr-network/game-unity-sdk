using MirageSDK.Core.Data;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.Examples.UseCases.AddSwitchNetwork
{
	public class AddSwitchNetwork : MonoBehaviour
	{
		[SerializeField]
		private Button _bscButton;

		[SerializeField]
		private Button _bscTestButton;

		private void Awake()
		{
			_bscButton.onClick.AddListener(OpenAddSwitchBsc);
			_bscTestButton.onClick.AddListener(OpenAddSwitchBscTestnet);
		}

		private void OnDestroy()
		{
			_bscButton.onClick.RemoveListener(OpenAddSwitchBsc);
			_bscTestButton.onClick.RemoveListener(OpenAddSwitchBscTestnet);
		}

		private static void OpenAddSwitchBsc()
		{
			MirageNetworkHelper.AddAndSwitchNetwork(NetworkName.BinanceSmartChain);
		}

		private static void OpenAddSwitchBscTestnet()
		{
			MirageNetworkHelper.AddAndSwitchNetwork(NetworkName.BinanceSmartChainTestNet);
		}
	}
}