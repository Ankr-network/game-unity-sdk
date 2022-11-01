using AnkrSDK.Aptos.Constants;
using AnkrSDK.Aptos.DTO;
using AnkrSDK.Aptos.Imlementation.ABI;
using UnityEngine;

namespace AnkrSDK.Aptos
{
	public class AptosTransaction : MonoBehaviour
	{
		private void Start()
		{
			Check();
		}

		public void SubmitTransaction()
		{
			var sender = "0xf38748cf83fad1ee1371e50a6b3d1ede95e7805c82ce43efd9948f4fe3533280";
			var transaction = new Transaction
			{
				Sender = sender,
				SequenceNumber = 0,
				MaxGasAmount = 200000,
				GasUnitPrice = 100,
				ExpirationTimestampSecs = 1666767052,
				Payload = new TransactionPayload
				{
					Type = "entry_function_payload",
					Function = "0x1::aptos_coin::transfer",
					TypeArguments = new string[] { "0x1::aptos_coin::AptosCoin" },
					Arguments = new string[]
					{
						"0xa6f6f770ee027db6eacd677e493bc9c2ae5392b47c48e5adfdb7094337e47b9a",
						"1000"
					},
				}
			};

			var signature = new TransactionSignature
			{
				Type = "ed25519_signature",
				PublicKey = sender,
				// Signature = SerializePayload(transaction)
			};

			transaction.Signature = signature;
			
			
		}

		// private string SerializePayload(Transaction transaction)
		// {
		// 	var sender = SerializeUtils.SerializeString(transaction.Sender);
		// 	var sequenceNumber = SerializeUtils.SerializeUint64(transaction.SequenceNumber);
		// 	
		// }

		public void Check()
		{
			var builder = new TransactionBuilderABI(ABIs.GetCoinABIs());
		}

		public string ShowArray<T>(T[] bytes)
		{
			var lalal = "";

			for (int i = 0; i < bytes.Length; i++)
			{
				lalal += bytes[i] + " ";
			}
			
			return lalal;
		}
	}
}