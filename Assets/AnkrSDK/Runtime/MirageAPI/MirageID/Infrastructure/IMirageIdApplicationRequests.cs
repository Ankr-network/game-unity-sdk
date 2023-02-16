using AnkrSDK.MirageAPI.Data;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.MirageAPI.MirageID.Infrastructure
{
	public interface IMirageIdApplicationRequests
	{
		bool IsInitialized();
		UniTask<string> Initialize(MirageAPISettingsSO settingsSO);
		UniTask<string> Initialize(string clientId, string clientSecret);
		UniTask Logout();

		UniTask<string> CreateUser(
			string email,
			string username,
			string firstName = null,
			string lastName = null
		);
	}
}