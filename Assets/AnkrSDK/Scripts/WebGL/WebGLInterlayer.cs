#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;

namespace AnkrSDK.WebGL
{
	public static class WebGLInterlayer
	{
		[DllImport("__Internal")]
		private static extern void SendTransaction(string id, string payload);

		[DllImport("__Internal")]
		private static extern void SignMessage(string id, string payload);

		[DllImport("__Internal")]
		private static extern void EstimateGas(string id, string payload);

		[DllImport("__Internal")]
		private static extern void GetAddresses(string id);

		[DllImport("__Internal")]
		private static extern void GetTransaction(string id, string transactionHash);

		[DllImport("__Internal")]
		private static extern void GetTransactionReceipt(string id, string transactionHash);

		[DllImport("__Internal")]
		public static extern string GetResponses();

		public static string SendTransaction(string payload)
		{
			var id = GenerateId();
			SendTransaction(id, payload);
			return id;
		}

		public static string SignMessage(string payload)
		{
			var id = GenerateId();
			SignMessage(id, payload);
			return id;
		}

		public static string GetTransactionReceipt(string transactionHash)
		{
			var id = GenerateId();
			GetTransactionReceipt(id, transactionHash);
			return id;
		}

		public static string GetTransaction(string transactionHash)
		{
			var id = GenerateId();
			GetTransaction(id, transactionHash);
			return id;
		}

		public static string GetAddresses()
		{
			var id = GenerateId();
			GetAddresses(id);
			return id;
		}

		public static string EstimateGas(string payload)
		{
			var id = GenerateId();
			EstimateGas(id, payload);
			return id;
		}

		private static string GenerateId()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
#endif