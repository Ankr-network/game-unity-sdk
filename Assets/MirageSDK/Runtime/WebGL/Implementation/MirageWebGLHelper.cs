using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;

namespace MirageSDK.WebGL.Implementation
{
	public class MirageWebGLHelper : INetworkHelper
	{
		private readonly WebGLWrapper _webGlWrapper;
		
		public MirageWebGLHelper(WebGLWrapper webGlWrapper)
		{
			_webGlWrapper = webGlWrapper;
		}

		public UniTask AddAndSwitchNetwork(EthChainData chain)
		{
			return _webGlWrapper.SwitchChain(chain);
		}
	}
}