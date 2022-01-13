using UnityEngine;
using UnityEngine.UI;
using Web3Unity.Scripts.Library;

public class AccountLinkingExample : MonoBehaviour
{
    [SerializeField]
	private string message = "Hahaha!";
	private string signature;
	private Web3 web3;
    [SerializeField]
    private Text address;
	
	private void Start()
	{
		string provider_url = "https://rinkeby.infura.io/v3/c75f2ce78a4a4b64aa1e9c20316fda3e";
		
		web3 = new Web3(provider_url);
		web3.Initialize();
	}

	public async void Sign()
	{
		Debug.Log(web3);
		signature = await web3.Sign(message);
		Debug.Log($"Signature: {signature}");

        ShowAdressinUI();
	}
	
	public void ShowAdressinUI()
	{
        address.text = signature;
	}
}
