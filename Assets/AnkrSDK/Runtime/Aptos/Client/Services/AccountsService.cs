using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Aptos.Services
{
	public class AccountsService : BaseService
	{
		private const string Root = "/accounts";
		
		private readonly string _getAccountRoute = $@"{Root}/{{0}}";
		private readonly string _getAccountResourcesRoute = $@"{Root}/{{0}}/resources";
		private readonly string _getAccountResourceByTypeRoute = $@"{Root}/{{0}}/resource/{{1}}";
		private readonly string _getAccountQuery = @"?ledger_version={1}";
		
		public AccountsService(OpenAPIConfig config) : base(config)
		{
		}

		public UniTask<AccountData> GetAccount(string address, ulong? ledgerVersion = null)
		{
			var url = URL + string.Format(_getAccountRoute, address);
			if (ledgerVersion != null)
			{
				url += string.Format(_getAccountQuery, ledgerVersion);
			}
			return WebHelper.SendGetRequest<AccountData>(url);
		}
		
		public UniTask<MoveResource<TData>> GetAccountResource<TData>(string address, string resourceType, ulong? ledgerVersion = null)
		{
			var url = URL + string.Format(_getAccountResourceByTypeRoute, address, resourceType);
			if (ledgerVersion != null)
			{
				url += string.Format(_getAccountQuery, ledgerVersion);
			}
			return WebHelper.SendGetRequest<MoveResource<TData>>(url);
		}
	}
}