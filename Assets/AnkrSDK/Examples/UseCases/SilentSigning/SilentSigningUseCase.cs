using System;
using AnkrSDK.Base;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Provider;
using AnkrSDK.WearableNFTExample;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.SilentSigning
{
	public class SilentSigningUseCase : UseCase
	{
		[SerializeField] private Button _requestSilentSignButton;
		[SerializeField] private Button _sendSilentSignTxButton;
		[SerializeField] private Button _disconnectSilentSignButton;
		[SerializeField] private TMP_Text _sessionInfoText;

		private IAnkrSDK _ankrSDK;
		private IContract _gameCharacterContract;
		private ISilentSigningSessionHandler _silentSigningSecretSaver;

		public override void ActivateUseCase()
		{
			base.ActivateUseCase();

			_ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(NetworkName.Goerli);
			_silentSigningSecretSaver = _ankrSDK.SilentSigningHandler.SessionHandler;
			_gameCharacterContract = _ankrSDK.GetContract(
				WearableNFTContractInformation.GameCharacterContractAddress,
				WearableNFTContractInformation.GameCharacterABI
			);
		}

		private void UpdateSessionInfoText()
		{
			_sessionInfoText.text = _silentSigningSecretSaver.IsSessionSaved()
				? _silentSigningSecretSaver.GetSavedSessionSecret()
				: "No Active session";
		}


		private void OnEnable()
		{
			_requestSilentSignButton.onClick.AddListener(OnRequestSilentSignClicked);
			_disconnectSilentSignButton.onClick.AddListener(OnDisconnectSilentSignClicked);
			_sendSilentSignTxButton.onClick.AddListener(OnSendSilentSignTxButtonClicked);
			_silentSigningSecretSaver.SessionUpdated += UpdateSessionInfoText;
			UpdateSessionInfoText();
		}

		private void OnDisable()
		{
			_silentSigningSecretSaver.SessionUpdated -= UpdateSessionInfoText;
			_requestSilentSignButton.onClick.RemoveAllListeners();
			_disconnectSilentSignButton.onClick.RemoveAllListeners();
			_sendSilentSignTxButton.onClick.RemoveAllListeners();
		}

		private void OnRequestSilentSignClicked()
		{
			var timeStamp = new DateTimeOffset(DateTime.UtcNow).AddDays(1).ToUnixTimeSeconds();
			Debug.Log("[SS] OnRequestSilentSignClicked");
			UniTask.Create(async () =>
			{
				var result = await _ankrSDK.SilentSigningHandler.RequestSilentSign(timeStamp).AsUniTask();

				Debug.Log(result);
			}).Forget();
		}

		private void OnDisconnectSilentSignClicked()
		{
			_ankrSDK.SilentSigningHandler.DisconnectSilentSign().AsUniTask().Forget();
		}

		private void OnSendSilentSignTxButtonClicked()
		{
			const string safeMintMethodName = "safeMint";

			Debug.Log("[SS] OnSendSilentSignTxButtonClicked");
			UniTask.Create(async () =>
			{
				var defaultAccount = await _ankrSDK.Eth.GetDefaultAccount();
				var transactionHash =
					await _gameCharacterContract.CallMethod(safeMintMethodName, new object[] { defaultAccount });

				Debug.Log($"[SS] Game Character Minted. Hash : {transactionHash}");
			}).Forget();
		}
	}
}