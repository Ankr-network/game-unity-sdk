using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Unity.Network;
using Object = UnityEngine.Object;

namespace Tests
{
	public class WalletConnectTests
	{
		[UnityTest]
		public IEnumerator WalletConnect_ConnectedOnStart()
		{
			var walletConnectObject = new GameObject("WalletConnect");
			var walletConnect = walletConnectObject.AddComponent<WalletConnect>();
			walletConnectObject.AddComponent<NativeWebSocketTransport>();
			walletConnect.connectOnStart = false;
			walletConnect.connectOnAwake = false;
			walletConnect.AppData = new ClientMeta
			{
				Description = "Wallet",
				URL = "https://www.ankr.com/",
				Icons = new[] { "https://www.ankr.com/static/favicon/apple-touch-icon.png" },
				Name = "Wallet"
			};

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
					throw;
				}
			});

			Assert.That(walletConnect.Session.ReadyForUserPrompt);

			Object.DestroyImmediate(walletConnectObject);
		}
	}
}