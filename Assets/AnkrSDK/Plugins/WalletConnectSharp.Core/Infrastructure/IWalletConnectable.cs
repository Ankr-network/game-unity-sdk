using System;
using System.Collections.Generic;
using AnkrSDK.WalletConnectSharp.Core.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Core.Infrastructure
{
    public interface IWalletConnectable
    { 
        string SettingsFilename { get; }
        void Initialize(ScriptableObject settings);
	    UniTask Connect();
    }
}
