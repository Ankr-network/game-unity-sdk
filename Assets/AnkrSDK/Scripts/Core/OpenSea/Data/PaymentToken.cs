namespace AnkrSDK.Core.OpenSea.Data
{
	public class PaymentToken
	{
		public int id { get; set; }
		public string symbol { get; set; }
		public string address { get; set; }
		public string image_url { get; set; }
		public string name { get; set; }
		public int decimals { get; set; }
		public double? eth_price { get; set; }
		public double? usd_price { get; set; }
	}
}