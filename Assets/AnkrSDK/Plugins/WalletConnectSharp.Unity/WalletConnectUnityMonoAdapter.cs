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
			_quittables.Add(walletConnect);
			_pausables.Add(walletConnect);
			_updatables.Add(walletConnect.Transport);
			_pausables.Add(walletConnect.Transport);
		}

		private void Awake()
		{
			if (_existingInstance != null)
			{
				Destroy(gameObject);
			}
			
			DontDestroyOnLoad(gameObject);

			_existingInstance = this;
		}

		private void Update()
		{
			_updatables.ForEach(_ => _.Update());
		}

		private async void OnApplicationPause(bool pauseStatus)
		{
			await Task.WhenAll(_pausables.Select( p => p.OnApplicationPause(pauseStatus)));
		}

		private async void OnApplicationQuit()
		{
			await Task.WhenAll(_quittables.Select( q => q.Quit()));
		}

		private async void OnDestroy()
		{
			await Task.WhenAll(_quittables.Select( q => q.Quit()));
		}
	}
}