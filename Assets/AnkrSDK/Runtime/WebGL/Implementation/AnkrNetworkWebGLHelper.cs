using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WebGL.Implementation
{
	public class AnkrNetworkWebGLHelper : INetworkHelper
	{
		private readonly WebGLWrapper _webGlWrapper;
		
		public AnkrNetworkWebGLHelper(WebGLWrapper webGlWrapper)
		{
			_webGlWrapper = webGlWrapper;
		}

		public UniTask AddAndSwitchNetwork(EthereumNetwork network)
		{
			return _webGlWrapper.SwitchChain(network);
		}
	}
}