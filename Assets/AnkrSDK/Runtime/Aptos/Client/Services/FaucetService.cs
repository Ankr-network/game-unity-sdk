using AnkrSDK.Aptos.DTO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.Aptos.Services
{
	public class FaucetService
	{
		private const string FundAccountRoute = "/mint";

		private readonly string _url;
		
		public FaucetService(string faucetUrl)
		{
			_url = faucetUrl;
		}

		public UniTask<IndexResponse> FundAccount(Account account, uint amount)
		{
			return WebHelper.SendPostRequest<IndexResponse>(_url + FundAccountRoute);
		}
	}
}