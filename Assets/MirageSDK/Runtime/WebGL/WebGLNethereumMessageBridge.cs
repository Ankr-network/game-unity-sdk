using MirageSDK.WebGL.Data;
using MirageSDK.WebGL.Infrastructure;
using UnityEngine;

namespace MirageSDK.WebGL
{
	public class WebGLNethereumMessageBridge : MonoBehaviour
	{
		private IWebGLNethereumCallbacksReceiver _callbackReceiver;
		public CallbackFallbackPair EnableEthereumCallbackNames => new CallbackFallbackPair
		{
			CallbackName = nameof(EthereumEnabled),
			FallbackName = nameof(DisplayError)
		};

		public CallbackFallbackPair GetChainIdCallbackNames => new CallbackFallbackPair
		{
			CallbackName = nameof(ChainChanged),
			FallbackName = nameof(DisplayError)
		};

		public void SetProtocol(IWebGLNethereumCallbacksReceiver protocol)
		{
			_callbackReceiver = protocol;
		}
		public void EthereumEnabled(string addressSelected)
		{
			_callbackReceiver.EthereumEnabled(addressSelected);
		}

		public void NewAccountSelected(string accountAddress)
		{
			_callbackReceiver.NewAccountSelected(accountAddress);
		}

		public void ChainChanged(string chainId)
		{
			_callbackReceiver.ChainChanged(chainId);
		}

		public void DisplayError(string errorMessage)
		{
			_callbackReceiver.DisplayError(errorMessage);
		}

		public void DisconnetedDummy(string str)
		{

		}
	}
}