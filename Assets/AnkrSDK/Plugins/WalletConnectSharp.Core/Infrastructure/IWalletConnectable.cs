using System.Collections.Generic;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Core.Infrastructure
{
    public interface IWalletConnectable
    { 
        string SettingsFilename { get; }
        void Initialize(ScriptableObject settings);
    }
}
