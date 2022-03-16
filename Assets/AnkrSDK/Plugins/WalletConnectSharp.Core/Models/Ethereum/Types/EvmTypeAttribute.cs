using System;

namespace AnkrSDK.WalletConnectSharp.Core.Models.Ethereum.Types
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