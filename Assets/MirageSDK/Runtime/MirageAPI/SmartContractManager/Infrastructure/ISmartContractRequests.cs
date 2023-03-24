using System.Collections.Generic;
using MirageSDK.MirageAPI.SmartContractManager.Data.AllContracts;
using MirageSDK.MirageAPI.SmartContractManager.Data.GetContract;
using Cysharp.Threading.Tasks;

namespace MirageSDK.MirageAPI.SmartContractManager.Infrastructure
{
	public interface ISmartContractRequests
	{
		public bool IsInitialized();
		public void SetToken(string applicationToken);
		public UniTask<GetContractResponseDTO> GetContractInfo(string contractId);
		public UniTask<List<SCMContractIDPair>> GetAllContracts();
	}
}