using System;
using System.Runtime.Serialization;

namespace AnkrSDK.Aptos
{
	[Serializable]
	public class AptosException : InvalidOperationException, ISerializable
	{
		public string Message { get; private set; }
		public string ErrorCode { get; private set; }
		public int VmErrorCode { get; private set; }

		public static string CreateMessage(string message, string errorCode, int vmErrorCode)
		{
			return $"Aptos error {errorCode}: {message} (vm_error_code: {vmErrorCode}).";
		}
		
		public AptosException(string message, string errorCode, int vmErrorCode) : base(CreateMessage(message, errorCode, vmErrorCode))
		{
			Message = message;
			ErrorCode = errorCode;
			VmErrorCode = vmErrorCode;
		}
		
		protected AptosException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
		
		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData(serializationInfo, streamingContext);
		}
		
		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData(serializationInfo, streamingContext);
		}
	}
}