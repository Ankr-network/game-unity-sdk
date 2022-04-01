using System;

namespace AnkrSDK.Core.OpenSea.Helpers
{
	public static class OrderByNameHelper
	{
		public static string GetNameByType(this NFTOrderByType orderByType)
		{
			switch (orderByType)
			{
				case NFTOrderByType.None:
					return string.Empty;
				case NFTOrderByType.PK:
					return "pk";
				case NFTOrderByType.SaleDate:
					return "sale_date";
				case NFTOrderByType.SaleCount:
					return "sale_count";
				case NFTOrderByType.SalePrice:
					return "sale_price";
				default:
					throw new ArgumentOutOfRangeException(nameof(orderByType), orderByType, null);
			}
		}
	}
}