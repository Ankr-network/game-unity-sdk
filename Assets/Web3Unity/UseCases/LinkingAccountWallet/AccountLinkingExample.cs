using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Utility.Utils;
using Web3Unity.Scripts.Library;

public class RequestPayload
{
	public string message;
	public string signature;
}

public class RequestAnswer
{
	public string Address;
}

public class AccountLinkingExample : MonoBehaviour
{
    [SerializeField]
	private string message = "Hahaha!";
	private string signature;
	private Web3 web3;
    [SerializeField]
    private Text address;

	private string url = "http://2.56.91.78:8080/account/verification/address";
	
	private void Start()
	{
		string provider_url = "https://rinkeby.infura.io/v3/c75f2ce78a4a4b64aa1e9c20316fda3e";
		
		web3 = new Web3(provider_url);
		web3.Initialize();
	}

	public async void Sign()
	{
		signature = await web3.Sign(message);
		Debug.Log($"Signature: {signature}");

		string address = await SendSignatue(signature);
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
		
		string payload = JsonConvert.SerializeObject(requestPayload);
		
		UnityWebRequest webRequest = WebRequests.SendJSON(url, payload);
		await webRequest.SendWebRequest();
		RequestAnswer data = JsonConvert.DeserializeObject<RequestAnswer>(webRequest.downloadHandler.text);
		return data.Address;
	}
}
