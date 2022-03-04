using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using UnityEngine;

namespace MirageSDK.UseCases.Add_SwitchNetwork
{
	public class AddSwitchNetwork : MonoBehaviour
	{
		public void OpenAddSwitchBsc()
		{
			MirageSDKWrapper.GetSDKInstance().AddAndSwitchNetwork(NetworkName.BinanceSmartChain);
		}
		
		public void OpenAddSwitchBscTestnet()
		{
			MirageSDKWrapper.GetSDKInstance().AddAndSwitchNetwork(NetworkName.BinanceSmartChainTestNet);
		}
	}
}