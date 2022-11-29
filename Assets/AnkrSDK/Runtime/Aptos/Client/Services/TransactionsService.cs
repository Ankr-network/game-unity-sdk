using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Aptos.Services
{
	public class TransactionsService : BaseService
	{
		private const string EstimateGasPriceRoute = "/estimate_gas_price";
		private const string SubmitTransactionRoute = "/transactions";
		private const string GetTransactionByHashRoute = @"/transactions/by_hash/{0}";
		private const string JsonWrapperForTransaction = @"{{""transaction"":{0}}}";
		
		public TransactionsService(OpenAPIConfig config) : base(config)
		{
		}
		
		public UniTask<PendingTransaction> SubmitTransaction(SubmitTransactionRequest requestBody)
		{
			return WebHelper.SendPostRequest<SubmitTransactionRequest, PendingTransaction>(URL + SubmitTransactionRoute, requestBody);
		}
		
		public async UniTask<TypedTransaction> GetTransactionByHash(string hash)
		{
			var url = URL + string.Format(GetTransactionByHashRoute, hash);
			var wrapper = await WebHelper.SendGetRequest<WrappedTransaction>(url, wrapper: JsonWrapperForTransaction);
			return wrapper.Transaction;
		}
		
		public UniTask<GasEstimation> EstimateGasPrice()
		{
			return WebHelper.SendGetRequest<GasEstimation>(URL + EstimateGasPriceRoute);
		}
	}
}