using System;
using AnkrSDK.Base;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Provider;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.SilentSigning
{
	public class SilentSigningUseCase : UseCase
	{
		[SerializeField] private Button _requestSilentSignButton;
		[SerializeField] private Button _sendSilentSignTxButton;
		[SerializeField] private Button _disconnectSilentSignButton;

		private IAnkrSDK _ankrSDK;
		private IContract _gameCharacterContract;

		public override void ActivateUseCase()
		{
			base.ActivateUseCase();

			_ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(NetworkName.Goerli);
		}

		private void OnEnable()
		{
			_requestSilentSignButton.onClick.AddListener(OnRequestSilentSignClicked);
			_disconnectSilentSignButton.onClick.AddListener(OnDisconnectSilentSignClicked);
			_sendSilentSignTxButton.onClick.AddListener(OnSendSilentSignTxButtonClicked);
		}

		private void OnDisable()
		{
			_requestSilentSignButton.onClick.RemoveAllListeners();
			_disconnectSilentSignButton.onClick.RemoveAllListeners();
			_sendSilentSignTxButton.onClick.RemoveAllListeners();
		}

		private void OnRequestSilentSignClicked()
		{
			var timeStamp = new DateTimeOffset(DateTime.UtcNow).AddDays(1).ToUnixTimeSeconds();
			_ankrSDK.SilentSigningHandler.RequestSilentSign(timeStamp).AsUniTask().Forget();
		}

		private void OnDisconnectSilentSignClicked()
		{
			_ankrSDK.SilentSigningHandler.DisconnectSilentSign().AsUniTask().Forget();
		}

		private async void OnSendSilentSignTxButtonClicked()
		{
			const string safeMintMethodName = "safeMint";

			var defaultAccount = await _ankrSDK.Eth.GetDefaultAccount();
			var transactionHash =
				await _gameCharacterContract.CallMethod(safeMintMethodName, new object[] { defaultAccount });

			Debug.Log($"Game Character Minted. Hash : {transactionHash}");
		}
	}
}