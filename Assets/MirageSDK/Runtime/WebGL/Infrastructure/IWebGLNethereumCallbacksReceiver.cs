namespace MirageSDK.WebGL.Infrastructure
{
	public interface IWebGLNethereumCallbacksReceiver
	{
		void EthereumEnabled(string addressSelected);
		void NewAccountSelected(string accountAddress);
		void ChainChanged(string chainId);
		void DisplayError(string errorMessage);
	}
}