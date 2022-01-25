using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Utility.Utils;
using Web3Unity.Core;

public class RequestPayload
{
	public string message;
	public string signature;
}

public class RequestAnswer
{
	public string Address;
}

// This example is to demonstrate how to bind the wallet into user account. 
public class AccountLinkingExample : MonoBehaviour
{
    [SerializeField]
        // Message to be signed, which should be provided by the server
	private string message = "Hahaha!";
	private string signature;
	private Web3 web3;
    [SerializeField]
    	private Text address;

	// A backend server to verfiy the ownership of this address using message and signature signed by 3rd party wallet
	// an example can be found at https://github.com/mirage-xyz/mirage-go-demo/blob/main/main.go#L96
	private string url = "http://2.56.91.78:8080/account/verification/address";
	
	private void Start()
	{
		var provider_url = "https://rinkeby.infura.io/v3/c75f2ce78a4a4b64aa1e9c20316fda3e";
		
		web3 = new Web3(provider_url);
		// Get the credential info from the 3rd party wallet
		web3.Initialize();
	}
	
	// function to sign the message
	// step 1: sign the message with 3rd party wallet
	// step 2: send the message and sign to the server 
	// step 3: server return binded address 
	public async void Sign()
	{
		signature = await web3.Sign(message);
		Debug.Log($"Signature: {signature}");
		
		var address = await SendSignatue(signature);
		Debug.Log($"Answer: {address}");

        	ShowAdressinUI();
	}
	
	public void ShowAdressinUI()
	{
                address.text = signature;
	}

	private async Task<string> SendSignatue(string signature)
	{
		var requestPayload = new RequestPayload
		{
			message = message,
			signature = signature
		};
		
		var payload = JsonConvert.SerializeObject(requestPayload);
		
		var webRequest = WebRequests.SendJSON(url, payload);
		await webRequest.SendWebRequest();
		var data = JsonConvert.DeserializeObject<RequestAnswer>(webRequest.downloadHandler.text);
		return data.Address;
	}
}
