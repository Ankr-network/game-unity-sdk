using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
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

		public void OpenAddSwitchBsc()
		{
			MirageSDKWrapper.GetSDKInstance().AddAndSwitchNetwork(NetworkNameEnum.BinanceSmartChain);
		}

		public void OpenAddSwitchBscTestnet()
		{
			MirageSDKWrapper.GetSDKInstance().AddAndSwitchNetwork(NetworkNameEnum.BinanceSmartChainTestNet);
		}
	}
}