using UnityEngine;
using Web3Unity.Core;

public class SignatureExample : MonoBehaviour
{
	private string message = "Hahaha!";
	private string signature;
	private Web3 web3;
	
	private void Start()
	{
		string provider_url = "https://rinkeby.infura.io/v3/c75f2ce78a4a4b64aa1e9c20316fda3e";
		
		web3 = new Web3(provider_url);
		web3.Initialize();
	}
	
	public void Sign1()
	{
		Debug.Log($"Signature: {signature}");
	}

	public async void Sign()
	{
		Debug.Log(web3);
		signature = await web3.Sign(message);
		Debug.Log($"Signature: {signature}");
	}
	
	public void CheckSignature()
	{
		string address = web3.CheckSignature(message, signature);
		Debug.Log($"Address from signature: {address}");
	}
}
