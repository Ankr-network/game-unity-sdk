using System;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	[CreateAssetMenu(fileName = "WalletConnect2Settings", menuName = "AnkrSDK/WalletConnect/WalletConnect2Settings")]
	public class WalletConnect2SettingsSO : ScriptableObject
	{
		public string ProjectId { get; set; }
		public string Description { get; set; }
		public string[] Icons { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }

		private void OnEnable()
		{
			
		}

	}
}