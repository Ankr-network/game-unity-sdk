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

		private MirageNftReader<MirageNftExample> _nftReader;
		private IEthHandler _ethHandler;

		public override void SetUseCaseBodyActive(bool isActive)
		{
			if (isActive)
			{
				var sdkInstance = MirageSDKFactory.GetMirageSDKInstance(_networkToUse);
				var ethHandler = sdkInstance.Eth;
				var mirageEpochErc721Contract = sdkInstance.GetContract(_mirageNftManagerContract);
				var contractReader = new MirageNftContractReader(mirageEpochErc721Contract, ethHandler);
				_nftReader = new MirageNftReader<MirageNftExample>(contractReader);
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
			var defaultAccount = await _ethHandler.GetDefaultAccount();
			sb.AppendLine($"NFTs owned by {defaultAccount}:");
			sb.AppendLine("");

			int nftsFound = 0;
			for (int i = 0; i < nftsList.Count; i++)
			{
				if (nftsList[i] == null)
				{
					continue;
				}

				sb.Append($"NFT {i}: " + nftsList[i] + "; ");
				nftsFound++;
			}

			if (nftsFound == 0)
			{
				sb.AppendLine(" none");
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