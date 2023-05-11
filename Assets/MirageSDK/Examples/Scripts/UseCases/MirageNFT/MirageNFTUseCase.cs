using System.Text;
using MirageSDK.Base;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.Provider;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.UseCases.MirageNFT
{
	public class MirageNFTUseCase : UseCaseBodyUI
	{
		[SerializeField] private Button _logMirageNftsButton;
		[SerializeField] private TMP_Text _logText;
		[SerializeField] private ContractInformationSO _mirageNftManagerContract;
		[SerializeField] private NetworkName _networkToUse;

		private MirageNftReader<MirageNftDataExample> _nftReader;
		private IEthHandler _ethHandler;

		public override void SetUseCaseBodyActive(bool isActive)
		{
			if (isActive)
			{
				var sdkInstance = MirageSDKFactory.GetMirageSDKInstance(_networkToUse);
				var ethHandler = sdkInstance.Eth;
				var mirageEpochErc721Contract = sdkInstance.GetContract(_mirageNftManagerContract);
				var contractReader = new MirageNftContractReader(mirageEpochErc721Contract, ethHandler);
				_nftReader = new MirageNftReader<MirageNftDataExample>(contractReader);
				_ethHandler = ethHandler;
			}
			
			base.SetUseCaseBodyActive(isActive);
		}
		
		private void Awake()
		{
			_logMirageNftsButton.onClick.AddListener(LogMirageNft);
		}

		private void OnDestroy()
		{
			_logMirageNftsButton.onClick.RemoveListener(LogMirageNft);
		}

		private async void LogMirageNft()
		{
			var nftsList = await _nftReader.GetNftsList();
			var sb = new StringBuilder();
			sb.AppendLine($"NFTs owned by {await _ethHandler.GetDefaultAccount()}:");
			sb.AppendLine("");
			for (int i = 0; i < nftsList.Count; i++)
			{
				sb.Append($"NFT {i}: " + nftsList[i] + "; ");
			}
			
			UpdateUILogs(sb.ToString());
		}

		private void UpdateUILogs(string log)
		{
			_logText.text += "\n" + log;
			Debug.Log(log);
		}
	}
}