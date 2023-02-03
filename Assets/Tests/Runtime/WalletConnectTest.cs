using System;
using System.Collections;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Unity;
using AnkrSDK.WalletConnectSharp.Unity.Utils;
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
							UniTask.WaitUntil(() => connect.Status == WalletConnectStatus.SessionRequestSent),
							UniTask.Delay(TimeSpan.FromSeconds(5f)));
					}
					catch (Exception e)
					{
						Debug.LogError(e.Message);
					}
				});

				Assert.That(connect.Status == WalletConnectStatus.SessionRequestSent);
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
				Assert.IsNull(walletConnect.Status == WalletConnectStatus.Uninitialized);
			}
		}

		[Test]
		public void WalletConnect_SessionConnection()
		{
			
			using (var wc = WalletHelper.CreateWalletConnectObject())
			{
				wc.InitializeSession();
				Assert.IsFalse(wc.Status == WalletConnectStatus.Uninitialized);
				Assert.IsFalse(wc.Connecting);
				Assert.IsFalse(wc.Status == WalletConnectStatus.TransportConnected);
				Assert.IsFalse(wc.Status == WalletConnectStatus.SessionRequestSent);
				Assert.IsFalse(wc.Status == WalletConnectStatus.WalletConnected);
			}
		}
	}
}