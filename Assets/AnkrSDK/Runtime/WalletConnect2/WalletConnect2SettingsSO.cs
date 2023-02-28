using System.Collections.Generic;
using UnityEngine;

namespace AnkrSDK.WalletConnect2
{
	[CreateAssetMenu(fileName = "WalletConnect2Settings", menuName = "AnkrSDK/WalletConnect/WalletConnect2Settings")]
	public class WalletConnect2SettingsSO : ScriptableObject
	{
		public string ProjectId => _projectId;
		public string Description => _description;
		public string[] Icons => _icons;
		public string Name => _name;
		public string Url => _url;
		public string DappFileName => _dappFileName;

		public IEnumerable<BlockchainParameters> BlockchainParameters
		{
			get
			{
				if (_blockChainParameters != null)
				{
					foreach (var blockchainParameter in _blockChainParameters)
					{
						if(blockchainParameter.Enabled)
							yield return blockchainParameter;
					}
				}
			}
		}
		
		[SerializeField] private string _projectId;
		[SerializeField] private string _description;
		[SerializeField] private string[] _icons;
		[SerializeField] private string _name;
		[SerializeField] private string _url;
		[SerializeField] private string _dappFileName;
		[SerializeField] private BlockchainParameters[] _blockChainParameters;


		private void OnEnable()
		{
			if (string.IsNullOrWhiteSpace(_projectId))
			{
				_projectId = null;
			}
			
			if (string.IsNullOrWhiteSpace(_description))
			{
				_description = null;
			}

			if (_icons != null && _icons.Length != 0)
			{
				for (int i = 0; i < _icons.Length; i++)
				{
					if (string.IsNullOrWhiteSpace(_icons[i]))
					{
						_icons[i] = null;
					}
				}
			}
		}

	}
}