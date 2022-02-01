using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Core.Utils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

// This example is to demonstrate how to bind the wallet into user account. 
namespace MirageSDK.UseCases.LinkingAccountWallet
{
	public class AccountLinkingExample : MonoBehaviour
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

		// Message to be signed, which should be provided by the server
		[SerializeField] private string _message = "Hahaha!";

		[SerializeField] private Text _address;

		private string _signature;

		private IMirageSDK _mirageSDKWrapper;


		// A backend server to verify the ownership of this address using message and signature signed by 3rd party wallet
		// an example can be found at https://github.com/mirage-xyz/mirage-go-demo/blob/main/main.go#L96
		private const string URL = "http://2.56.91.78:8080/account/verification/address";

		private void Start()
		{
			const string providerURL = "https://rinkeby.infura.io/v3/c75f2ce78a4a4b64aa1e9c20316fda3e";

			_mirageSDKWrapper = MirageSDKWrapper.GetInitializedInstance(providerURL);
		}

		// function to sign the message
		// step 1: sign the message with 3rd party wallet
		// step 2: send the message and sign to the server 
		// step 3: server return binded address 
		public async void Sign()
		{
			_signature = await _mirageSDKWrapper.Sign(_message);
			Debug.Log($"Signature: {_signature}");

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

			var webRequest = MirageSDKHelpers.SendJSON(URL, payload);
			await webRequest.SendWebRequest();
			var data = JsonConvert.DeserializeObject<RequestAnswer>(webRequest.downloadHandler.text);
			return data.Address;
		}
	}
}