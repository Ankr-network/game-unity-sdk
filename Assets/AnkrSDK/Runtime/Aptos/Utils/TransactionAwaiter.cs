using System;
using System.Threading;
using AnkrSDK.Aptos.Constants;
using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.Aptos
{
	public class TransactionAwaiter
	{
		private CancellationTokenSource _cancellationTokenSource;
		private Client _client;
		
		public TransactionAwaiter(Client client)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_client = client;
		}
		
		public async UniTask<TypedTransaction> WaitForTransactionWithResult(string hash)
		{
			var isPending = true;
			var token = _cancellationTokenSource.Token;
			TypedTransaction transaction = null;
			while (!_cancellationTokenSource.IsCancellationRequested && isPending)
			{
				try
				{
					transaction = await _client.GetTransactionByHash(hash);
					
					if (transaction.Type == TransactionTypes.PendingTransaction)
					{
						await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
					}
					else
					{
						isPending = false;
					}
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
					break;
				}
			}

			return transaction;
		}

		public void StopWait()
		{
			_cancellationTokenSource.Cancel();
		}
	}
}