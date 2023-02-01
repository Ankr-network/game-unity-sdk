using System;
using System.Collections;
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

			using (var walletConnect = new WalletConnect.WalletConnectTestWrapper(WalletHelper.CreateWalletConnectObject()))
			{
				yield return UniTask.ToCoroutine(async () =>
				{
					try
					{
						await UniTask.WhenAny(
							walletConnect.Connect().AsUniTask(),
							//TODO ANTON update tests
							//UniTask.WaitUntil(() => walletConnect.Session.ReadyForUserPrompt),
							UniTask.Delay(TimeSpan.FromSeconds(5f)));
					}
					catch (Exception e)
					{
						Debug.LogError(e.Message);
					}
				});

				//TODO ANTON update tests
				//Assert.That(walletConnect.Session.ReadyForUserPrompt);
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
			using (var walletConnect = new WalletConnect.WalletConnectTestWrapper(WalletHelper.CreateWalletConnectObject()))
			{
				//TODO ANTON update tests
				//Assert.IsNull(walletConnect.Session);
			}
		}

		[Test]
		public void WalletConnect_SessionConnection()
		{
			using (var walletConnect = new WalletConnect.WalletConnectTestWrapper(WalletHelper.CreateWalletConnectObject()))
			{
				walletConnect.InitializeUnitySession();
				//TODO ANTON update tests
				// var session = walletConnect.Session;
				// Assert.IsNotNull(session);
				// Assert.IsFalse(session.Connected);
				// Assert.IsFalse(session.Connecting);
				// Assert.IsFalse(session.Disconnected);
				// Assert.IsFalse(session.SessionUsed);
			}
		}
	}
}