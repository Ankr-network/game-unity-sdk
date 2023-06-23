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
			FirstCallbackName = nameof(EthereumEnabled),
			SecondCallbackName = nameof(DisplayError)
		};

		public CallbackFallbackPair EthereumInitCallbackNames => new CallbackFallbackPair
		{
			FirstCallbackName = nameof(NewAccountSelected),
			SecondCallbackName = nameof(ChainChanged)
		};

		public CallbackFallbackPair GetChainIdCallbackNames => new CallbackFallbackPair
		{
			FirstCallbackName = nameof(ChainChanged),
			SecondCallbackName = nameof(DisplayError)
		};

		public CallbackFallbackPair RequestCallbackNames => new CallbackFallbackPair
		{
			FirstCallbackName = nameof(HandleRpcResponse),
			SecondCallbackName = nameof(DisplayError)
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

		public void HandleRpcResponse(string rpcResponse)
		{
			_callbackReceiver.HandleRpcResponse(rpcResponse);
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