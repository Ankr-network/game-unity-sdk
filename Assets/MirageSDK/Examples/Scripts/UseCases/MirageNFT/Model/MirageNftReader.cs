using System;
using System.Collections.Generic;
using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Utils;
using UnityEngine;

namespace MirageSDK.UseCases.MirageNFT
{
	public class MirageNftReader<T> where T : MirageNftBase, new()
	{
		private readonly MirageNftContractReader _contractReader;

		public MirageNftReader(MirageNftContractReader contractReader)
		{
			_contractReader = contractReader;
		}

		public async UniTask<IReadOnlyList<T>> GetNftsList()
		{
			var ownedNftIds = await _contractReader.GetOwnedNftIds();
			var tasksList = new List<UniTask<T>>();
			foreach (var tokenId in ownedNftIds)
			{
				tasksList.Add(GetNfts(tokenId));
			}
			
			var dtosArray = await UniTask.WhenAll(tasksList);
			return dtosArray;
		}

		private async UniTask<T> GetNfts(BigInteger tokenId)
		{
			var imageUriTask = _contractReader.GetUriFromNftId(tokenId);
			var uri = await imageUriTask;
			
			if (string.IsNullOrWhiteSpace(uri))
			{
				Debug.LogError("uri not found for " + tokenId);
				return null;
			}
			
			MirageNftDto dto = null;
			try
			{
				dto = await WebHelper.SendGetRequest<MirageNftDto>(uri);
			}
			catch (UnityWebRequestException e)
			{
				Debug.LogError($"UnityWebRequestException {e.Message} {e.StackTrace}");
				return null;
			}

			var resultInstance = Activator.CreateInstance<T>();
			resultInstance.Init(dto);
			return resultInstance;

		}
	}
}