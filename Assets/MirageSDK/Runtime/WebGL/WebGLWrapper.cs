using MirageSDK.Core.Infrastructure;
using Cysharp.Threading.Tasks;
using MirageSDK.Data;

namespace MirageSDK.WebGL
{
	public class WebGLWrapper : IWalletHandler
	{
		private readonly WebGLConnect _connect;

		public WebGLWrapper(WebGLConnect webGlConnect)
		{
			_connect = webGlConnect;
		}

		public UniTask Disconnect(bool waitForNewSession = true)
		{
			return _connect.CloseSession(waitForNewSession);
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			return _connect.GetWalletsStatus();
		}
	}
}