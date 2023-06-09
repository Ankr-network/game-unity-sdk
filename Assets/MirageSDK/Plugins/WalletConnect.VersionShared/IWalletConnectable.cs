using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageSDK.WalletConnect.VersionShared
{
	public interface IWalletConnectable
	{
		string WalletName { get; }
		UniTask<string> GetDefaultAccount(string network = null);
		event Action<string[]> OnAccountChanged;
		string SettingsFilename { get; }
		void Initialize(ScriptableObject settings);
		UniTask Connect();
		UniTask CloseSession(bool connectNewSession = true);
		UniTask<string> ReconnectSession();
	}
}