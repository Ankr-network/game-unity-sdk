using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.WalletConnect.VersionShared
{
	public interface IWalletConnectable
	{
		string SettingsFilename { get; }
		void Initialize(ScriptableObject settings);
		UniTask Connect();
	}
}