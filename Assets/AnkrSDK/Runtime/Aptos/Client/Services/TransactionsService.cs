using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Aptos.Services
{
	public class TransactionsService : BaseService
	{
		private const string EstimateGasPriceRoute = "/estimate_gas_price";
		private const string SubmitTransactionRoute = "/transactions";
		private const string GetTransactionByHashRoute = @"/transactions/by_hash/{0}";
		
		public TransactionsService(OpenAPIConfig config) : base(config)
		{
		}
		
		public UniTask<PendingTransaction> SubmitTransaction(SubmitTransactionRequest1 requestBody)
		{
			return WebHelper.SendPostRequest<SubmitTransactionRequest1, PendingTransaction>(URL + SubmitTransactionRoute, requestBody);
		}
		
		public UniTask<WrappedTransaction> GetTransactionByHash(string hash)
		{
			return WebHelper.SendGetRequest<WrappedTransaction>(URL + string.Format(GetTransactionByHashRoute, hash), wrapper: @"{{""transaction"":{0}}}");
		}
		
		public UniTask<GasEstimation> EstimateGasPrice()
		{
			return WebHelper.SendGetRequest<GasEstimation>(URL + EstimateGasPriceRoute);
		}
	}
}