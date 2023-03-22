using System;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum.Types
{
    public class EvmTypeAttribute : Attribute
    {
        public string TypeName { get; }
        
        public EvmTypeAttribute(string typename)
        {
            TypeName = typename;
        }
    }
}