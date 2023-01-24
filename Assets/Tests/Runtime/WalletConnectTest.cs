using System;
using System.Collections;
using AnkrSDK.WalletConnectSharp.Core.Models;
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
				yield return UniTask.ToCoroutine(async () =>
				{
					try
					{
						await UniTask.WhenAny(
							walletConnect.Connect().AsUniTask(),
							UniTask.WaitUntil(() => walletConnect.Session.ReadyForUserPrompt),
							UniTask.Delay(TimeSpan.FromSeconds(5f)));
					}
					catch (Exception e)
					{
						Debug.LogError(e.Message);
					}
				});

				Assert.That(walletConnect.Session.ReadyForUserPrompt);
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
				Assert.IsNull(walletConnect.Session);
			}
		}

		[Test]
		public void WalletConnect_SessionConnection()
		{
			using (var walletConnect = WalletHelper.CreateWalletConnectObject())
			{
				walletConnect.InitializeUnitySession();
				var session = walletConnect.Session;
				Assert.IsNotNull(session);
				Assert.IsFalse(session.Connected);
				Assert.IsFalse(session.Connecting);
				Assert.IsFalse(session.Disconnected);
				Assert.IsFalse(session.SessionUsed);
			}
		}
	}
}