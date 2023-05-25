using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageSDK.WalletConnect.VersionShared
{
	public interface IWalletConnectable
	{
		string SettingsFilename { get; }
		void Initialize(ScriptableObject settings);
		UniTask Connect();
		UniTask CloseSession(bool connectNewSession = true);
	}
}