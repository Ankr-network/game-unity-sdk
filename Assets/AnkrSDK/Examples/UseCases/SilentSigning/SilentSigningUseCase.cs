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

		private void OnSendSilentSignTxButtonClicked()
		{
			_ankrSDK.SilentSigningHandler.SendSilentTransaction().AsUniTask().Forget();
		}
	}
}