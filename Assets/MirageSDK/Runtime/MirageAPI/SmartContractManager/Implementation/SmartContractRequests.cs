using System.Collections.Generic;
using MirageSDK.MirageAPI.SmartContractManager.Data.AllContracts;
using MirageSDK.MirageAPI.SmartContractManager.Data.GetContract;
using MirageSDK.MirageAPI.SmartContractManager.Helpers;
using MirageSDK.MirageAPI.SmartContractManager.Infrastructure;
using MirageSDK.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageSDK.MirageAPI.SmartContractManager.Implementation
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