using System.Runtime.InteropServices;

namespace MirageSDK.WebGL
{
	public static class WebGLInterlayer
	{
		[DllImport("__Internal")]
		public static extern void CreateProvider(string id, string payload);
		
		[DllImport("__Internal")]
		public static extern void GetWalletsStatus(string id);
		
		[DllImport("__Internal")]
		public static extern void SendTransaction(string id, string payload);

		[DllImport("__Internal")]
		public static extern void SignMessage(string id, string payload);

		[DllImport("__Internal")]
		public static extern void GetContractData(string id, string payload);

		[DllImport("__Internal")]
		public static extern void EstimateGas(string id, string payload);

		[DllImport("__Internal")]
		public static extern void GetAddresses(string id);
		
		[DllImport("__Internal")]
		public static extern void RequestChainId(string id);

		[DllImport("__Internal")]
		public static extern void GetTransaction(string id, string transactionHash);

		[DllImport("__Internal")]
		public static extern void GetTransactionReceipt(string id, string transactionHash);
		
		[DllImport("__Internal")]
		public static extern void AddChain(string id, string networkData);
		
		[DllImport("__Internal")]
		public static extern void SwitchChain(string id, string networkData);
		
		[DllImport("__Internal")]
		public static extern void GetEvents(string id, string filters);
		
		[DllImport("__Internal")]
		public static extern void CallMethod(string id, string callObject);

		[DllImport("__Internal")]
		public static extern string GetResponses();
	}
}
