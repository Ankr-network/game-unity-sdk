using System.Collections.Generic;
using AnkrSDK.MirageAPI.SmartContractManager.Data.AllContracts;
using AnkrSDK.MirageAPI.SmartContractManager.Data.GetContract;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.MirageAPI.SmartContractManager.Infrastructure
{
	public interface ISmartContractRequests
	{
		public bool IsInitialized();
		public void SetToken(string applicationToken);
		public UniTask<GetContractResponseDTO> GetContractInfo(string contractId);
		public UniTask<List<SCMContractIDPair>> GetAllContracts();
	}
}