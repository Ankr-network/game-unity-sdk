using System.Collections.Generic;

namespace AnkrSDK.Core.OpenSea.Data
{
	public class OpenSeaAsset
	{
		public int id { get; set; }
		public int num_sales { get; set; }
		public object background_color { get; set; }
		public object image_url { get; set; }
		public object image_preview_url { get; set; }
		public object image_thumbnail_url { get; set; }
		public object image_original_url { get; set; }
		public object animation_url { get; set; }
		public object animation_original_url { get; set; }
		public object name { get; set; }
		public object description { get; set; }
		public object external_link { get; set; }
		public ContractAsset asset_contract { get; set; }
		public string permalink { get; set; }
		public Collection collection { get; set; }
		public int decimals { get; set; }
		public object token_metadata { get; set; }
		public bool is_nsfw { get; set; }
		public Owner owner { get; set; }
		public object sell_orders { get; set; }
		public object creator { get; set; }
		public List<object> traits { get; set; }
		public object last_sale { get; set; }
		public object top_bid { get; set; }
		public object listing_date { get; set; }
		public bool is_presale { get; set; }
		public object transfer_fee_payment_token { get; set; }
		public object transfer_fee { get; set; }
		public List<object> related_assets { get; set; }
		public List<object> orders { get; set; }
		public List<object> auctions { get; set; }
		public bool supports_wyvern { get; set; }
		public List<Ownership> top_ownerships { get; set; }
		public Ownership ownership { get; set; }
		public object highest_buyer_commitment { get; set; }
		public string token_id { get; set; }
	}
}