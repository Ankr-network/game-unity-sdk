using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Unity.UI
{
	public class PlatformToggle : MonoBehaviour
	{
		public bool _activeOnDesktop;
		public bool _activeOnAndroid;
		public bool _activeOniOS;

		private void Start()
		{
			MakeActive();
		}

		private void MakeActive()
		{
		#if UNITY_ANDROID
			gameObject.SetActive(_activeOnAndroid);
		#elif UNITY_IOS
        gameObject.SetActive(_activeOniOS);
		#else
        gameObject.SetActive(_activeOnDesktop);
		#endif
		}
	}
}
