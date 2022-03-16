﻿using System;
using System.IO;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Unity.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace AnkrSDK.WalletConnectSharp.Unity.Models
{
	public class MessageSigner : MonoBehaviour
	{
		public static MessageSigner CreateMessageSigner(GameObject parent, string messageToSign, int accountIndex = 0,
			bool disconnectOnSign = false, string addressToSign = null)
		{
			Debug.LogWarning("[WalletConnect] CreateMessageSigner is experimental, proceed with caution");
			
			var signer = parent.AddComponent<MessageSigner>();
			signer.messageToSign = messageToSign;
			signer.accountIndex = accountIndex;
			signer.disconnectOnSign = disconnectOnSign;
			signer.overrideAddressToSign = addressToSign;

			return signer;
		}
		
		public static async Task<WCMessageSigned> SignAndDisconnect(GameObject parent, string messageToSign, int accountIndex = 0, string addressToSign = null)
		{
			Debug.LogWarning("[WalletConnect] SignAndDisconnect is experimental, proceed with caution");
			
			var signer = parent.AddComponent<MessageSigner>();
			signer.messageToSign = messageToSign;
			signer.accountIndex = accountIndex;
			signer.disconnectOnSign = true;
			signer.overrideAddressToSign = addressToSign;

			await signer.gameObject.WaitForStart();

			var result = await signer.SignMessage();
			
			Destroy(signer);

			return result;
		}
		
		[Serializable]
		public class WCMessageSignedEvent : UnityEvent<WCMessageSigned>
		{
		}

		public string messageToSign;
		public string overrideAddressToSign;
		public int accountIndex = 0;
		public bool disconnectOnSign;

		public event EventHandler<WCMessageSigned> MessageSigned;

		public WCMessageSignedEvent OnMessageSigned;

		public async void OnClickSignMessage()
		{
			await SignMessage();
		}

		public async Task<WCMessageSigned> SignMessage()
		{
			if (!WalletConnect.ActiveSession.Connected)
			{
				throw new IOException("[WalletConnect] No ActiveSession is connected to sign the message");
			}

			var address = WalletConnect.ActiveSession.Accounts[accountIndex];

			if (!string.IsNullOrEmpty(overrideAddressToSign))
			{
				address = overrideAddressToSign;
			}

			var result = await WalletConnect.ActiveSession.EthPersonalSign(address, messageToSign);
			
			//Lets recover the signer

			var eventData = new WCMessageSigned(this, result);

			if (MessageSigned != null)
				MessageSigned(this, eventData);
			
			OnMessageSigned.Invoke(eventData);

			if (disconnectOnSign)
			{
				await WalletConnect.ActiveSession.Disconnect();
			}

			return eventData;
		}
	}
}
