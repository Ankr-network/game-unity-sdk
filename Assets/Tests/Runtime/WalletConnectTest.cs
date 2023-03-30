using System;
using System.Collections;
using MirageSDK.WalletConnectSharp.Core;
using MirageSDK.WalletConnectSharp.Core.Models;
using MirageSDK.WalletConnectSharp.Unity.Utils;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Runtime
{
	public class WalletConnectTests
	{
		[UnityTest]
		public IEnumerator WalletConnect_ConnectedOnStart()
		{
			SavedSession savedSessionBeforeTest = null;
			if (SessionSaveHandler.IsSessionSaved())
			{
				savedSessionBeforeTest = SessionSaveHandler.GetSavedSession();
			}

			SessionSaveHandler.ClearSession();

			using (var walletConnect = WalletHelper.CreateWalletConnectObject())
			{
				var connect = walletConnect;
				yield return UniTask.ToCoroutine(async () =>
				{
					try
					{
						await UniTask.WhenAny(
							connect.Connect(),
							UniTask.WaitUntil(() => connect.Status == WalletConnectStatus.SessionRequestSent),
							UniTask.Delay(TimeSpan.FromSeconds(5f)));
					}
					catch (Exception e)
					{
						Debug.LogError(e.Message);
					}
				});

				Assert.AreEqual(WalletConnectStatus.SessionRequestSent, connect.Status);
			}

			if (savedSessionBeforeTest != null)
			{
				SessionSaveHandler.SaveSession(savedSessionBeforeTest);
			}
			else
			{
				SessionSaveHandler.ClearSession();
			}
		}

		[Test]
		public void WalletConnect_DefaultSessionIsUnInitialized()
		{
			using (var walletConnect = WalletHelper.CreateWalletConnectObject())
			{
				Assert.AreEqual(WalletConnectStatus.Uninitialized,walletConnect.Status);
			}
		}

		[Test]
		public void WalletConnect_SessionConnection()
		{
			using (var wc = WalletHelper.CreateWalletConnectObject())
			{
				wc.InitializeSession();
				var walletConnectStatus = wc.Status;
				Assert.AreNotEqual(WalletConnectStatus.Uninitialized, walletConnectStatus);
				Assert.IsFalse(wc.Connecting);
				Assert.AreNotEqual(WalletConnectStatus.TransportConnected,walletConnectStatus);
				Assert.AreNotEqual(WalletConnectStatus.SessionRequestSent,walletConnectStatus);
				Assert.AreNotEqual(WalletConnectStatus.WalletConnected,walletConnectStatus);
			}
		}
	}
}