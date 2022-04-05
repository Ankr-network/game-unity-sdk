using System;
using System.Collections.Generic;

namespace AnkrSDK.Core.OpenSea.Data
{
	public class Collection
	{
		public List<PaymentToken> payment_tokens { get; set; }
		public List<PrimaryAssetContract> primary_asset_contracts { get; set; }
		public Traits traits { get; set; }
		public Stats stats { get; set; }
		public object banner_image_url { get; set; }
		public object chat_url { get; set; }
		public DateTime created_date { get; set; }
		public bool default_to_fiat { get; set; }
		public object description { get; set; }
		public string dev_buyer_fee_basis_points { get; set; }
		public string dev_seller_fee_basis_points { get; set; }
		public object discord_url { get; set; }
		public DisplayData display_data { get; set; }
		public object external_url { get; set; }
		public bool featured { get; set; }
		public object featured_image_url { get; set; }
		public bool hidden { get; set; }
		public string safelist_request_status { get; set; }
		public object image_url { get; set; }
		public bool is_subject_to_whitelist { get; set; }
		public object large_image_url { get; set; }
		public object medium_username { get; set; }
		public string name { get; set; }
		public bool only_proxied_transfers { get; set; }
		public string opensea_buyer_fee_basis_points { get; set; }
		public string opensea_seller_fee_basis_points { get; set; }
		public object payout_address { get; set; }
		public bool require_email { get; set; }
		public object short_description { get; set; }
		public string slug { get; set; }
		public object telegram_url { get; set; }
		public object twitter_username { get; set; }
		public object instagram_username { get; set; }
		public object wiki_url { get; set; }
		public bool is_nsfw { get; set; }
	}
}