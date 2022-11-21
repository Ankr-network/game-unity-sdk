using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.Aptos.Services
{
	public class GeneralService : BaseService
	{
		private const string GetLedgerInfoRoute = "/";
		
		public GeneralService(OpenAPIConfig config) : base(config)
		{
		}

		public UniTask<IndexResponse> GetLedgerInfo()
		{
			return WebHelper.SendGetRequest<IndexResponse>(URL + GetLedgerInfoRoute);
		}
	}
}