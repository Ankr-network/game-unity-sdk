using System;
using System.Threading.Tasks;
using AnkrSDK.Core.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This example is to demonstrate how to bind the wallet into user account. 
namespace AnkrSDK.UseCases.LinkingAccountWallet
{
	public class AccountLinkingExample : UseCase
	{
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
		private const string URL = "http://root@eth-01.dccn.ankr.com:8080/account/verification/address";

		// Message to be signed, which should be provided by the server
		[SerializeField] private string _message = "Hahaha!";

		[SerializeField] private TMP_Text _address;
		
		[SerializeField] private Button _signLinkinMessageButton;

		private string _signature;
		
		private void Awake()
		{
			_signLinkinMessageButton.onClick.AddListener(Sign);
		}

		private void OnDestroy()
		{
			_signLinkinMessageButton.onClick.RemoveListener(Sign);
		}
		
		// function to sign the message
		// step 1: sign the message with 3rd party wallet
		// step 2: send the message and sign to the server 
		// step 3: server return bound address 
		private async void Sign()
		{
			_signature = await AnkrSignatureHelper.Sign(_message);
			Debug.Log($"Signature: {_signature}");
			_address.text = _signature;
			var address = await SendSignature(_signature);
			Debug.Log($"Answer: {address}");

			ShowAddressInUI();
		}

		private void ShowAddressInUI()
		{
			_address.text = _signature;
		}

		private async Task<string> SendSignature(string signature)
		{
			var requestPayload = new RequestPayload
			{
				Message = _message,
				Signature = signature
			};

			var payload = JsonConvert.SerializeObject(requestPayload);

			var result = await AnkrSDKHelper.GetUnityWebRequestFromJSON(URL, payload)
				.SendWebRequest();
			var data = JsonConvert.DeserializeObject<RequestAnswer>(result.downloadHandler.text);
			return data.Address;
		}
	}
}