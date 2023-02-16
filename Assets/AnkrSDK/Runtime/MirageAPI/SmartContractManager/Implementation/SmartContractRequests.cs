using System.Collections.Generic;
using AnkrSDK.MirageAPI.SmartContractManager.Data.AllContracts;
using AnkrSDK.MirageAPI.SmartContractManager.Data.GetContract;
using AnkrSDK.MirageAPI.SmartContractManager.Helpers;
using AnkrSDK.MirageAPI.SmartContractManager.Infrastructure;
using AnkrSDK.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.MirageAPI.SmartContractManager.Implementation
{
	public class SmartContractRequests : ISmartContractRequests
	{
		private string _applicationToken;

		public bool IsInitialized()
		{
			return !string.IsNullOrEmpty(_applicationToken);
		}

		public void SetToken(string applicationToken)
		{
			_applicationToken = applicationToken;
		}

		public UniTask<GetContractResponseDTO> GetContractInfo(string contractId)
		{
			if (!IsInitialized())
			{
				return UniTask.FromResult<GetContractResponseDTO>(null);
			}

			var headers = SCMRequestsHelper.GetAuthorizationHeader(_applicationToken);
			var contractInfoURL = SCMEndpoints.GetContractInfoURL(contractId);
			Debug.Log($"Send Get to : {contractInfoURL}");
			return WebHelper.SendGetRequest<GetContractResponseDTO>(contractInfoURL,
				headers);
		}

		public UniTask<List<SCMContractIDPair>> GetAllContracts()
		{
			if (!IsInitialized())
			{
				return UniTask.FromResult<List<SCMContractIDPair>>(null);
			}

			var headers = SCMRequestsHelper.GetAuthorizationHeader(_applicationToken);
			var allContractsURL = SCMEndpoints.GetAllContractsURL;
			Debug.Log($"Send Get to : {allContractsURL}");
			return WebHelper.SendGetRequest<List<SCMContractIDPair>>(allContractsURL, headers);
		}
	}
}