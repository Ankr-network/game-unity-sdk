using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Unity;
using AnkrSDK.WalletConnectSharp.Unity.Network;
using AnkrSDK.WalletConnectSharp.Unity.Utils;
using NUnit.Framework;
using TestUtils;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace PlayModeTests
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

			var walletConnect = TestHelper.CreateWalletConnectObject();
			walletConnect.ConnectOnStart = true;
			yield return UniTask.ToCoroutine(async () =>
			{
				try
				{
					await UniTask.WhenAny(
						walletConnect.Connect().AsUniTask(),
						UniTask.WaitUntil(() => WalletConnect.ActiveSession.ReadyForUserPrompt),
						UniTask.Delay(TimeSpan.FromSeconds(5f)));
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
				}
			});

			Assert.That(WalletConnect.ActiveSession.ReadyForUserPrompt);

			Object.DestroyImmediate(walletConnect.gameObject);
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
			var walletConnect = TestHelper.CreateWalletConnectObject();
			Assert.IsNull(WalletConnect.ActiveSession);

			Object.DestroyImmediate(walletConnect.gameObject);
		}

		[Test]
		public void WalletConnect_SessionConnection()
		{
			var walletConnect = TestHelper.CreateWalletConnectObject();
			walletConnect.InitializeUnitySession();
			var session = WalletConnect.ActiveSession;
			Assert.IsNotNull(session);
			Assert.IsFalse(session.Connected);
			Assert.IsFalse(session.Connecting);
			Assert.IsFalse(session.Disconnected);
			Assert.IsFalse(session.SessionUsed);

			Object.DestroyImmediate(walletConnect.gameObject);
		}
	}
}