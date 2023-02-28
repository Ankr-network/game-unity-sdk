using System.Collections.Generic;
using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	public class WalletConnectUnityMonoAdapter : MonoBehaviour
	{
		private readonly List<IUpdatable> _updatables = new List<IUpdatable>();
		private readonly List<IPausable> _pausables = new List<IPausable>();
		private readonly List<IQuittable> _quittables = new List<IQuittable>();

		private WalletConnectUnityMonoAdapter _existingInstance;

		public void Clear()
		{
			_updatables.Clear();
			_pausables.Clear();
			_quittables.Clear();
		}

		public void TryAddObject(object someObject)
		{
			if (someObject is IUpdatable updatable)
			{
				_updatables.Add(updatable);
			}

			if (someObject is IQuittable quittable)
			{
				_quittables.Add(quittable);
			}

			if (someObject is IPausable pausable)
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
			await UniTask.WhenAll(_pausables.Select(p => p.OnApplicationPause(pauseStatus)));
		}

		private async void OnApplicationQuit()
		{
			await UniTask.WhenAll(_quittables.Select(q => q.Quit()));
		}

		private async void OnDestroy()
		{
			await UniTask.WhenAll(_quittables.Select(q => q.Quit()));
		}
	}
}