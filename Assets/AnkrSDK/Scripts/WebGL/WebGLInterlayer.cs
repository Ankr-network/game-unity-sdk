using System.Runtime.InteropServices;

namespace AnkrSDK.WebGL
{
	public static class WebGLInterlayer
	{
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
		public static extern void GetTransaction(string id, string transactionHash);

		[DllImport("__Internal")]
		public static extern void GetTransactionReceipt(string id, string transactionHash);

		[DllImport("__Internal")]
		public static extern string GetResponses();
	}
}
