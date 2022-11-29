using MirageSDK.UseCases;
using MirageSDK.WearableNFTExample;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.OpenSea.Examples
{
	public class OpenSeaController : UseCase
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
			MirageOpenSea.OpenSingleAssetInfoLink(
				WearableNFTContractInformation.GameCharacterContractAddress,
				"2");
		}
	}
}