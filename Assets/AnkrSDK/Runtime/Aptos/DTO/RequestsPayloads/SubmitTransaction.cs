using Mirage.Aptos.Imlementation.ABI;
using Mirage.Aptos.Utils;
using UnityEngine;
using TransactionPayloadABI = Mirage.Aptos.Imlementation.ABI.TransactionPayload;

namespace AnkrSDK.Aptos.DTO
{
	public class SubmitTransaction
	{
		public Account Sender;
		public Account Receiver;
		public ulong Amount;
		public ulong SequenceNumber;
		public ulong MaxGasAmount;
		public ulong GasUnitPrice;
		public ulong ExpirationTimestampSecs;
		public TransactionPayloadABI Payload;
		public uint ChainID;

		public RawTransaction GetRaw()
		{
			return new RawTransaction(
				Sender.Address.HexToByteArray(),
				SequenceNumber,
				Payload,
				MaxGasAmount,
				GasUnitPrice,
				ExpirationTimestampSecs,
				ChainID
			);
		}
		
		public SubmitTransactionRequest GetRequest(TransactionPayload payload, TransactionSignature signature)
		{
			return new SubmitTransactionRequest
			{
				Sender = Sender.Address,
				SequenceNumber = SequenceNumber.ToString(),
				MaxGasAmount = MaxGasAmount.ToString(),
				GasUnitPrice = GasUnitPrice.ToString(),
				ExpirationTimestampSecs = ExpirationTimestampSecs.ToString(),
				Payload = payload,
				Signature = signature,
			};
		}
	}
}