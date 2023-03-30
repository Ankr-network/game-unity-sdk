using MirageSDK.Data;
using UnityEngine;

namespace MirageSDK.WebGL
{   
    [CreateAssetMenu(fileName = "WebGLConnectSettings", menuName = "MirageSDK/WalletConnect/WebGLConnectSettings")]
    public class WebGLConnectSettingsSO : ScriptableObject
    {
        [SerializeField] private Wallet _defaultWallet = Wallet.None;
        [SerializeField] private NetworkName _defaultNetwork = NetworkName.Goerli;

        public Wallet DefaultWallet => _defaultWallet;
        public NetworkName DefaultNetwork => _defaultNetwork;
    }
}