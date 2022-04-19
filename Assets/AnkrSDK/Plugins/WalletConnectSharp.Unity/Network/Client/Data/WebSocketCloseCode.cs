namespace AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data
{
	public enum WebSocketCloseCode
	{
		/* Do NOT use NotSet - it's only purpose is to indicate that the close code cannot be parsed. */
		NotSet = 0,
		Normal = 1000,
		Away = 1001,
		ProtocolError = 1002,
		UnsupportedData = 1003,
		Undefined = 1004,
		NoStatus = 1005,
		Abnormal = 1006,
		InvalidData = 1007,
		PolicyViolation = 1008,
		TooBig = 1009,
		MandatoryExtension = 1010,
		ServerError = 1011,
		TlsHandshakeFailure = 1015
	}
}