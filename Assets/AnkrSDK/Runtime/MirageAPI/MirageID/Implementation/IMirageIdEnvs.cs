namespace AnkrSDK.MirageAPI.MirageID.Implementation
{
	public interface IMirageIdEnv
	{
		string TokenEndpoint { get; }
		string LogoutEndpoint { get; }
		string CreateUserURL { get; }
		string WalletInfoURL { get; }
	}
}