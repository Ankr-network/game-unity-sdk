using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Unity.Infrastructure;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	public class WalletConnectUnityMonoAdapter : MonoBehaviour
	{
		private readonly List<IUpdatableComponent> _updatables = new List<IUpdatableComponent>();
		private readonly List<IPausableComponent> _pausables = new List<IPausableComponent>();
		private readonly List<IQuittableComponent> _quittables = new List<IQuittableComponent>();

		private WalletConnectUnityMonoAdapter _existingInstance;

		public void SetupWalletConnect(WalletConnect walletConnect)
		{
			_updatables.Add(walletConnect.Transport);
			_pausables.Add(walletConnect.Transport);
			
			_pausables.Add(walletConnect);
			_quittables.Add(walletConnect);
		}

		private void Awake()
		{
			if (_existingInstance != null)
			{
				Destroy(gameObject);
				return;
			}
			
			DontDestroyOnLoad(gameObject);

			_existingInstance = this;
		}

		private void Update()
		{
			foreach (var updatable in _updatables)
			{
				updatable.Update();
			}
		}

		private async void OnApplicationPause(bool pauseStatus)
		{
			foreach (var pausable in _pausables)
			{
				await pausable.OnApplicationPause(pauseStatus);
			}
		}

		private async void OnApplicationQuit()
		{
			foreach (var quittable in _quittables)
			{
				await quittable.Quit();
			}
		}

		private async void OnDestroy()
		{
			foreach (var quittable in _quittables)
			{
				await quittable.Quit();
			}
		}
	}
}