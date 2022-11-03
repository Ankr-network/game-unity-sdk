namespace AnkrSDK.Aptos.DTO
{
	public class Token
	{
		public string Type { get; set; }
		public string Value { get; set; }

		public Token(string type, string value)
		{
			Type = type;
			Value = value;
		}
	}
}