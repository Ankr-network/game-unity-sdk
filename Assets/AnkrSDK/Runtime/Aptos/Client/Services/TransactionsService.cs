using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Aptos.Services
{
	public class TransactionsService : BaseService
	{
		private const string EstimateGasPriceRoute = "/estimate_gas_price";
		private const string SubmitTransactionRoute = "/transactions";
		
		public TransactionsService(OpenAPIConfig config) : base(config)
		{
		}
		
		public UniTask<PendingTransaction<EntryFunctionPayload, Ed25519Signature>> SubmitTransaction(SubmitTransactionRequest1<EntryFunctionPayload, Ed25519Signature> requestBody)
		{
			return WebHelper.SendPostRequest<SubmitTransactionRequest1<EntryFunctionPayload, Ed25519Signature>, PendingTransaction<EntryFunctionPayload, Ed25519Signature>>(URL + SubmitTransactionRoute, requestBody);
		}
		
		public UniTask<GasEstimation> EstimateGasPrice()
		{
			return WebHelper.SendGetRequest<GasEstimation>(URL + EstimateGasPriceRoute);
		}
	}
}