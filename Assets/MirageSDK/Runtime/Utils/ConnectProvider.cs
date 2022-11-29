using System;
using UnityEngine;

namespace MirageSDK.Utils
{
	public static class ConnectProvider<T> where T : MonoBehaviour
	{
		private static T _connectProvider;

		public static T GetWalletConnect()
		{
			if (_connectProvider != null)
			{
				return _connectProvider;
			}

			_connectProvider = UnityEngine.Object.FindObjectOfType<T>();

			if (_connectProvider == null)
			{
				throw new ArgumentNullException(nameof(_connectProvider),
					$"Couldn't find a valid {nameof(T)} Instance on scene. Please make sure you have an instance ready to be used");
			}

			return _connectProvider;
		}
	}
}