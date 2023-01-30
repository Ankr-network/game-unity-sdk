using AnkrSDK.Data;
using UnityEngine;

namespace AnkrSDK.WebGL
{   
    [CreateAssetMenu(fileName = "WebGLConnectSettings", menuName = "AnkrSDK/WalletConnect/WebGLConnectSettings")]
    public class WebGLConnectSettingsSO : ScriptableObject
    {
        [SerializeField] private Wallet _defaultWallet = Wallet.None;
        [SerializeField] private NetworkName _defaultNetwork = NetworkName.Goerli;

        public Wallet DefaultWallet => _defaultWallet;
        public NetworkName DefaultNetwork => _defaultNetwork;
    }
}