using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using Cysharp.Threading.Tasks;

namespace MirageSDK.WebGL.Implementation
{
	public class MirageWebGLHelper : INetworkHelper
	{
		private readonly WebGLWrapper _webGlWrapper;
		
		public MirageWebGLHelper(WebGLWrapper webGlWrapper)
		{
			_webGlWrapper = webGlWrapper;
		}

		public UniTask AddAndSwitchNetwork(EthereumNetwork network)
		{
			return _webGlWrapper.SwitchChain(network);
		}
	}
}