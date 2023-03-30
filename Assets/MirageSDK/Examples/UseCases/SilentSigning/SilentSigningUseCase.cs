using System;
using MirageSDK.Base;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.Provider;
using MirageSDK.WearableNFTExample;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.UseCases.SilentSigning
{
	public class SilentSigningUseCase : UseCaseBodyUI
	{
		[SerializeField]
		private Button _requestSilentSignButton;

		[SerializeField]
		private Button _sendSilentSignTxButton;

		[SerializeField]
		private Button _disconnectSilentSignButton;

		[SerializeField]
		private TMP_Text _sessionInfoText;

		[SerializeField]
		private TMP_Text _sessionLogs;
		
		[SerializeField]
		private ContractInformationSO _gameCharacterContractInfo;

		private IMirageSDK _sdkInstance;
		private IContract _gameCharacterContract;
		private ISilentSigningSessionHandler _silentSigningSecretSaver;

		public override void SetUseCaseBodyActive(bool isActive)
		{
			if (isActive)
			{
				_sdkInstance = MirageSDKFactory.GetMirageSDKInstance(NetworkName.Goerli);
				_silentSigningSecretSaver = _sdkInstance.SilentSigningHandler.SessionHandler;

				_gameCharacterContract = _sdkInstance.GetContract(_gameCharacterContractInfo);
			}

			base.SetUseCaseBodyActive(isActive);
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


		private void UpdateSessionInfoText()
		{
			var isSessionSaved = _silentSigningSecretSaver.IsSessionSaved();
			_disconnectSilentSignButton.interactable = isSessionSaved;
			_sendSilentSignTxButton.interactable = isSessionSaved;
			_requestSilentSignButton.interactable = !isSessionSaved;
			_sessionInfoText.text = isSessionSaved
				? _silentSigningSecretSaver.GetSavedSessionSecret()
				: "No Active session";
		}

		private void OnRequestSilentSignClicked()
		{
			var timeStamp = new DateTimeOffset(DateTime.UtcNow).AddDays(1).ToUnixTimeSeconds();
			Debug.Log("[SS] OnRequestSilentSignClicked");
			UniTask.Create(async () =>
			{
				var result = await _sdkInstance.SilentSigningHandler.RequestSilentSign(timeStamp);

				Debug.Log(result);

				_sessionLogs.text = result + "\n" + _sessionLogs.text;
			}).Forget();
		}

		private void OnDisconnectSilentSignClicked()
		{
			_sdkInstance.SilentSigningHandler.DisconnectSilentSign().Forget();
		}

		private void OnSendSilentSignTxButtonClicked()
		{
			const string safeMintMethodName = "safeMint";

			Debug.Log("[SS] OnSendSilentSignTxButtonClicked");
			UniTask.Create(async () =>
			{
				var defaultAccount = await _sdkInstance.Eth.GetDefaultAccount();
				var transactionHash =
					await _gameCharacterContract.CallMethod(safeMintMethodName, new object[] { defaultAccount });

				var message = $"[SS] Game Character Minted. Hash : {transactionHash}";
				Debug.Log(message);
				_sessionLogs.text = message + "\n" + _sessionLogs.text;
			}).Forget();
		}
	}
}