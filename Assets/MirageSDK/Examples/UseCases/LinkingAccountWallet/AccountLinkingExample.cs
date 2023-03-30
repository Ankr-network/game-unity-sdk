using System;
using MirageSDK.Base;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.ERC20Example;
using MirageSDK.Provider;
using MirageSDK.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This example is to demonstrate how to bind the wallet into user account. 
namespace MirageSDK.UseCases.LinkingAccountWallet
{
	public class AccountLinkingExample : UseCaseBodyUI
	{
		[SerializeField] 
		private ProviderInformationSO _providerInfo;
		[Serializable]
		private class RequestPayload
		{
			public string Message;
			public string Signature;
		}

		[Serializable]
		private class RequestAnswer
		{
			public string Address;
		}


		// A backend server to verify the ownership of this address using message and signature signed by 3rd party wallet
		// an example can be found at https://github.com/mirage-xyz/mirage-go-demo/blob/main/main.go#L96
		private const string URL = "https://example-signing.game.ankr.com/account/verification/address";

		// Message to be signed, which should be provided by the server
		[SerializeField] private string _message = "Hahaha!";

		[SerializeField] private TMP_Text _text;

		[SerializeField] private Button _signLinkinMessageButton;
		[SerializeField] private Button _checkSignatureButton;

		private string _signature;
		private IEthHandler _eth;

		private void Awake()
		{
			_signLinkinMessageButton.onClick.AddListener(Sign);
			_checkSignatureButton.onClick.AddListener(CheckSignature);
		}

		private void OnDestroy()
		{
			_signLinkinMessageButton.onClick.RemoveListener(Sign);
			_checkSignatureButton.onClick.RemoveListener(CheckSignature);
		}

		public override void SetUseCaseBodyActive(bool isActive)
		{
			base.SetUseCaseBodyActive(isActive);

			if (isActive)
			{
				var sdkInstance = MirageSDKFactory.GetMirageSDKInstance(_providerInfo.HttpProviderURL);
				_eth = sdkInstance.Eth;
			}
		}

		// function to sign the message
		// step 1: sign the message with 3rd party wallet
		// step 2: send the message and sign to the server 
		// step 3: server return bound address 
		private async void Sign()
		{
			var address = await _eth.GetDefaultAccount();
			_signature = await _eth.Sign(_message, address);
			UpdateUILogs($"Signature: {_signature}");
		}

		private async void CheckSignature()
		{
			var address = await SendSignature(_signature);
			UpdateUILogs($"Answer: {address}");
		}

		private async UniTask<string> SendSignature(string signature)
		{
			var requestPayload = new RequestPayload
			{
				Message = _message,
				Signature = signature
			};

			var payload = JsonConvert.SerializeObject(requestPayload);

			var result = await MirageSDKHelper.GetUnityWebRequestFromJSON(URL, payload)
				.SendWebRequest();
			var data = JsonConvert.DeserializeObject<RequestAnswer>(result.downloadHandler.text);
			return data.Address;
		}

		private void UpdateUILogs(string log)
		{
			if (log == null)
			{
				log = "null string";
			}
			else if(log.Length == 0)
			{
				log = "empty string";
			}
			
			_text.text += "\n" + log;
			Debug.Log(log);
		}
	}
}