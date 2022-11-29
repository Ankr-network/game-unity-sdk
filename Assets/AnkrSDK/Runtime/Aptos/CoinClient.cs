using System.Numerics;
using AnkrSDK.Aptos.Constants;
using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;
using Mirage.Aptos.Constants;
using Mirage.Aptos.Imlementation.ABI;
using Mirage.Aptos.Utils;

namespace AnkrSDK.Aptos
{
	public class CoinClient
	{
		private const string AptosCoinType = "0x1::aptos_coin::AptosCoin";
		
		private Client _client;
		private TransactionBuilderABI _transactionBuilder;
		private Ed25519SignatureBuilder _signatureBuilder;

		public CoinClient(Client client)
		{
			_client = client;
			_transactionBuilder = new TransactionBuilderABI(ABIs.GetCoinABIs());
			_signatureBuilder = new Ed25519SignatureBuilder();
		}

		public async UniTask<string> Transfer(Account from, Account to, ulong amount)
		{
			var transaction = await PrepareTransaction(from, to, amount);
			var receipt = await _client.SubmitTransaction(transaction);
			return receipt.Hash;
		}

		private async UniTask<SubmitTransactionRequest> PrepareTransaction(Account from, Account to, ulong amount)
		{
			var transaction = await _client.GenerateTransactionRequest(from);
			transaction.Payload = GetPayload(to, amount);

			var raw = TransformRequestToRaw(transaction);

			transaction.Signature = _signatureBuilder.GetSignature(from, raw);

			return transaction;
		}

		private EntryFunctionPayload GetPayload(Account to, ulong amount)
		{
			return new EntryFunctionPayload
			{
				Type = TransactionPayloadTypes.EntryFunction,
				Function = "0x1::coin::transfer",
				TypeArguments = new string[] { AptosCoinType },
				Arguments = new string[] { to.Address, amount.ToString() }
			};
		}

		private RawTransaction TransformRequestToRaw(SubmitTransactionRequest transactionRequest)
		{
			var payload = (EntryFunctionPayload) transactionRequest.Payload;
			var rawPayload =
				_transactionBuilder.BuildTransactionPayload(payload.Function, payload.TypeArguments, payload.Arguments);
			return new RawTransaction(
				transactionRequest.Sender.HexToByteArray(),
				transactionRequest.SequenceNumber,
				rawPayload,
				transactionRequest.MaxGasAmount,
				transactionRequest.GasUnitPrice,
				transactionRequest.ExpirationTimestampSecs,
				transactionRequest.ChainID
			);
		}

		public async UniTask<BigInteger> GetBalance(Account account)
		{
			var typeTag = $"0x1::coin::CoinStore<{AptosCoinType}>";
			var resource = await _client.GetAccountResource(account, typeTag);
			
			var data = resource.Data.ToObject<CoinStoreType>();
			
			return BigInteger.Parse(data.Coin.Value);
		}
	}
}