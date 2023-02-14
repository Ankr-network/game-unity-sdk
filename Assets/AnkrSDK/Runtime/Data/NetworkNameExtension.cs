using System.ComponentModel;

namespace AnkrSDK.Data
{
    public static class NetworkNameExtension
    {
        public static int GetNetworkChainId(this NetworkName networkName)
        {
            switch (networkName)
            {
                case NetworkName.Mainnet:
                    return 1;
                case NetworkName.Ropsten:
                    return 3;
                case NetworkName.Rinkeby:
                    return 4;
                case NetworkName.Goerli:
                    return 5;
                case NetworkName.Kovan:
                    return 42;
            }

            throw new InvalidEnumArgumentException($"GetNetworkChainId is not implemented for {networkName}");
        }
    }
}