using UnityEngine;

namespace MirageSDK.UseCases.Add_SwitchNetwork
{
	public class AddSwitchNetwork : MonoBehaviour
	{
		private readonly string url = "dapp://change-network-mirage.surge.sh/";

		public void OpenAddSwitchUrl()
		{
			Application.OpenURL(url);
		}
	}
}