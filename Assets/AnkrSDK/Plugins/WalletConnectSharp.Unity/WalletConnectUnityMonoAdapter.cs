using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Infrastructure;
using AnkrSDK.WalletConnectSharp.Core.Models;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	public class WalletConnectUnityMonoAdapter : MonoBehaviour
	{
		private readonly List<IUpdatableComponent> _updatables = new List<IUpdatableComponent>();
		private readonly List<IPausableComponent> _pausables = new List<IPausableComponent>();
		private readonly List<IQuittableComponent> _quittables = new List<IQuittableComponent>();

		private WalletConnectUnityMonoAdapter _existingInstance;

		public void Clear()
		{
			_updatables.Clear();
			_pausables.Clear();
			_quittables.Clear();
		}

		public void TryAddObject(object someObject)
		{
			if (someObject is IUpdatableComponent updatable)
			{
				_updatables.Add(updatable);
			}
			
			if (someObject is IQuittableComponent quittable)
			{
				_quittables.Add(quittable);
			}
			
			if (someObject is IPausableComponent pausable)
			{
				_pausables.Add(pausable);
			}
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