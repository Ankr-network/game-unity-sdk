using System;
using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;
using Mirage.Aptos.Imlementation.ABI;
using Mirage.Aptos.Utils;
using UnityEngine;
using TransactionPayload = Mirage.Aptos.Imlementation.ABI.TransactionPayload;

namespace AnkrSDK.Aptos
{
	public class Client
	{
		private const int DefaultMaxGasAmount = 200000;
		private const int DefaultTxnExpSecFromNow = 20;

		private readonly ClientServices _services;

		public Client(string nodeUrl, OpenAPIConfig config = null)
		{
			if (config == null)
			{
				config = new OpenAPIConfig();
			}

			config.Base = nodeUrl;

			_services = new ClientServices(config);
		}

		public async UniTask<RawTransaction> GenerateRawTransaction(Account sender, TransactionPayload payload,
			OptionalTransactionArgs extraArgs = null)
		{
			var ledgerInfo = await _services.GeneralService.GetLedgerInfo();
			var account = await _services.AccountsService.GetAccount(sender.Address);
			var gasUnitPrice = extraArgs?.GasUnitPrice;
			if (gasUnitPrice == null)
			{
				var gasEstimation = await _services.TransactionsService.EstimateGasPrice();
				gasUnitPrice = gasEstimation.GasEstimate;
			}

			var expireTimestamp =
				(uint)Math.Floor((double)(new DateTime().Millisecond / 1000 + DefaultTxnExpSecFromNow));

			return new RawTransaction(
				sender.PublicKey.HexToByteArray(),
				account.SequenceNumber,
				payload,
				DefaultMaxGasAmount,
				(ulong)gasUnitPrice,
				expireTimestamp,
				ledgerInfo.ChainID
			);
		}

		public async UniTask<SubmitTransactionRequest<EntryFunctionPayload, Ed25519Signature>> GenerateTransactionRequest(Account sender,
			OptionalTransactionArgs extraArgs = null)
		{
			var ledgerInfo = await _services.GeneralService.GetLedgerInfo();
			var account = await _services.AccountsService.GetAccount(sender.Address);
			var gasUnitPrice = extraArgs?.GasUnitPrice;
			if (gasUnitPrice == null)
			{
				var gasEstimation = await _services.TransactionsService.EstimateGasPrice();
				gasUnitPrice = gasEstimation.GasEstimate;
			}

			var expireTimestamp =
				(uint)Math.Floor((double)(DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000 + DefaultTxnExpSecFromNow));

			return new SubmitTransactionRequest<EntryFunctionPayload, Ed25519Signature>
			{
				Sender = sender.Address,
				SequenceNumber = account.SequenceNumber,
				MaxGasAmount = DefaultMaxGasAmount,
				GasUnitPrice = (ulong)gasUnitPrice,
				ExpirationTimestampSecs = expireTimestamp,
				ChainID = ledgerInfo.ChainID
			};
		}

		public UniTask<PendingTransaction<EntryFunctionPayload, Ed25519Signature>> SubmitTransaction(SubmitTransactionRequest<EntryFunctionPayload, Ed25519Signature> request)
		{
			return _services.TransactionsService.SubmitTransaction(request);
		}

		public UniTask<MoveResource<T>> GetAccountResource<T>(Account account, string resourceType)
		{
			return this._services.AccountsService.GetAccountResource<T>(account.Address, resourceType);
		}
	}
}