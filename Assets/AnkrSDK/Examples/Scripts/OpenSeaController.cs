using AnkrSDK.Core.OpenSea;
using AnkrSDK.Examples.WearableNFTExample;
using AnkrSDK.UseCases;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK
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
			AnkrOpenSea.OpenSingleAssetInfoLink(
				WearableNFTContractInformation.GameCharacterContractAddress,
				"2");
		}
	}
}