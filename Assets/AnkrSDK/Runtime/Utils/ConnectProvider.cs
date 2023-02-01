using System;
using AnkrSDK.WalletConnectSharp.Core.Infrastructure;
using AnkrSDK.WalletConnectSharp.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AnkrSDK.Utils
{
	public static class ConnectProvider<TConnect, TSettings> where TConnect : IWalletConnectable, new() where TSettings : ScriptableObject
	{
		private static TConnect _instance;

		public static TConnect GetConnect()
		{
			if (_instance != null)
			{
				return _instance;
			}
			
			var connectAdapter = Object.FindObjectOfType<WalletConnectUnityMonoAdapter>();

			if (connectAdapter == null)
			{
				throw new ArgumentNullException(nameof(_instance),
					$"Couldn't find a valid {nameof(WalletConnectUnityMonoAdapter)} Instance on scene. Please make sure you have an instance ready to be used");
			}

			_instance = new TConnect();
			var settings = Resources.Load<TSettings>(_instance.SettingsFilename);
			_instance.Initialize(settings);
			connectAdapter.Clear();
			connectAdapter.TryAddObject(_instance);

			return _instance;
		}
	}
}