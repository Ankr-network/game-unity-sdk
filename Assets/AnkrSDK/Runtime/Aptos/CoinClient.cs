using System.Numerics;
using AnkrSDK.Aptos.Constants;
using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;
using Mirage.Aptos.Constants;
using Mirage.Aptos.Imlementation.ABI;
using Newtonsoft.Json;
using UnityEngine;
using TransactionPayloadABI = Mirage.Aptos.Imlementation.ABI.TransactionPayload;

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
			var payload = GetPayload(to, amount);
			var transaction = await PrepareTransaction(from, to, amount, payload);

			var raw = transaction.GetRaw();
			var signature = _signatureBuilder.GetSignature(from, raw);
			var request = transaction.GetRequest(payload, signature);

			Debug.Log(JsonConvert.SerializeObject(request));
			
			var receipt = await _client.SubmitTransaction(request);

			return receipt.Hash;
		}

		private async UniTask<SubmitTransaction> PrepareTransaction(
			Account from,
			Account to,
			ulong amount,
			EntryFunctionPayload payload
		)
		{
			var transaction = new SubmitTransaction
			{
				Sender = from,
				Receiver = to,
				Amount = amount
			};

			await _client.PopulateRequestParams(transaction);

			transaction.Payload = _transactionBuilder.BuildTransactionPayload(
				payload.Function,
				payload.TypeArguments,
				payload.Arguments
			);

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

		public async UniTask<BigInteger> GetBalance(Account account)
		{
			var typeTag = $"0x1::coin::CoinStore<{AptosCoinType}>";
			var resource = await _client.GetAccountResource(account, typeTag);

			var data = resource.Data.ToObject<CoinStoreType>();

			return BigInteger.Parse(data.Coin.Value);
		}
	}
}