namespace MirageSDK.Core.Infrastructure
{
	public interface IMirageSDK : IContractProvider, ISignatureProvider
	{
		void AddAndSwitchNetwork(NetworkNameEnum networkEnum);
		void AddAndSwitchCustomNetwork(string url);
	}
}