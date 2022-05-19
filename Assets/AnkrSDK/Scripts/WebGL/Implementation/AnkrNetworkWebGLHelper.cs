using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.WebGL;

namespace AnkrSDK.Utils
{
	public class AnkrNetworkWebGLHelper : INetworkHelper
	{
		private readonly WebGLWrapper _webGlWrapper;
		
		public AnkrNetworkWebGLHelper(WebGLWrapper webGlWrapper)
		{
			_webGlWrapper = webGlWrapper;
		}

		public Task AddAndSwitchNetwork(EthereumNetwork network)
		{
			return _webGlWrapper.SwitchChain(network);
		}
	}
}