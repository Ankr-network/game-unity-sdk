using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.OpenSea;
using AnkrSDK.Examples.WearableNFTExample;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK
{
	public class OpenSeaController : MonoBehaviour
	{
		[SerializeField] private Button _button;

		private void OnEnable()
		{
			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			_button.onClick.RemoveAllListeners();
		}

		private static void OnButtonClick()
		{
			ButtonClickedHandler().Forget();
		}

		private static async UniTaskVoid ButtonClickedHandler()
		{
			/*var singleAsset = await OpenSeaAPIAssets.GetSingleAsset(
				WearableNFTContractInformation.GameCharacterContractAddress,
				"2",
				EthHandler.DefaultAccount,
				true);
			Debug.Log(singleAsset);
			*/
			AnkrOpenSea.OpenSingleAssetInfoLink(
				WearableNFTContractInformation.GameCharacterContractAddress,
				"2");
		}
	}
}