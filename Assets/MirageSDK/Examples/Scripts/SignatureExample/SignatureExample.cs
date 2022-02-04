using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using UnityEngine;

namespace MirageSDK.Examples.Scripts.SignatureExample
{
	public class SignatureExample : MonoBehaviour
	{
		private const string Message = "Hahaha!";
		private string _signature;
		private IMirageSDK _mirageSDKWrapper;
	
		private void Start()
		{
			const string providerURL = "https://rinkeby.infura.io/v3/c75f2ce78a4a4b64aa1e9c20316fda3e";
		
			_mirageSDKWrapper = MirageSDKWrapper.GetInitializedInstance(providerURL);
		}
	
		public void Sign1()
		{
			Debug.Log($"Signature: {_signature}");
		}

		public async void Sign()
		{
			Debug.Log(_mirageSDKWrapper);
			_signature = await _mirageSDKWrapper.Sign(Message);
			Debug.Log($"Signature: {_signature}");
		}
	
		public void CheckSignature()
		{
			var address = _mirageSDKWrapper.CheckSignature(Message, _signature);
			Debug.Log($"Address from signature: {address}");
		}
	}
}
