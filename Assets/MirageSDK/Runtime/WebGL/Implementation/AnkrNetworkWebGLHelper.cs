using System.Threading.Tasks;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;

namespace MirageSDK.WebGL.Implementation
{
	public class MirageNetworkWebGLHelper : INetworkHelper
	{
		private readonly WebGLWrapper _webGlWrapper;
		
		public MirageNetworkWebGLHelper(WebGLWrapper webGlWrapper)
		{
			_webGlWrapper = webGlWrapper;
		}

		public Task AddAndSwitchNetwork(EthereumNetwork network)
		{
			return _webGlWrapper.SwitchChain(network);
		}
	}
}