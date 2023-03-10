using AnkrSDK.Base;
using AnkrSDK.WearableNFTExample;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.OpenSea.Examples
{
	public class OpenSeaController : UseCaseBodyUI
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
			AnkrOpenSea.OpenSingleAssetInfoLink(
				WearableNFTContractInformation.GameCharacterContractAddress,
				"2");
		}
	}
}