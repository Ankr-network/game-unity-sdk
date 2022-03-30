using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnkrSDK.Core.Utils
{
	public static class UriUtil
	{
		// Smallest possible number to match MonoAndroid version of Uri.EscapeDataString (value is hardcoded in method body)
		private const int UriMaxLength = 32766;

		// NOTE: when encoding data as application/x-www-form-urlencoded space characters are replaced by '+'
		//       http://www.w3.org/TR/html401/interact/forms.html#h-17.13.4.1
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">Data to encode</param>
		/// <param name="isFormData">If true space characters are encoded as '+', otherwise as "%20"</param>
		/// <returns></returns>
		public static string Encode(string data, bool isFormData = false)
		{
			if (string.IsNullOrEmpty(data))
			{
				return string.Empty;
			}

			if (data.Length < UriMaxLength)
			{
				var escaped = Uri.EscapeDataString(data);
				if (isFormData)
				{
					escaped = escaped.Replace("%20", "+");
				}

				return escaped;
			}

			var sb = new StringBuilder();
			for (var i = 0; i < data.Length; i += UriMaxLength)
			{
				var escaped = Uri.EscapeDataString(data.Substring(i, Math.Min(UriMaxLength, data.Length - i)));
				if (isFormData)
				{
					escaped = escaped.Replace("%20", "+");
				}

				sb.Append(escaped);
			}

			return sb.ToString();
		}

		public static string Decode(string data, bool isFormData = false)
		{
			if (isFormData)
			{
				data = data.Replace("+", "%20");
			}

			return Uri.UnescapeDataString(data);
		}

		public static Dictionary<string, string> ParseQueryString(string queryString, bool isFormData = false)
		{
			if (string.IsNullOrWhiteSpace(queryString))
			{
				return new Dictionary<string, string>();
			}

			if (queryString.Contains("?"))
			{
				queryString = queryString.Substring(queryString.IndexOf('?') + 1);
			}

			var dictionary = new Dictionary<string, string>();
			var pairs = queryString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
				.Where(kv => kv.Length == 2 && !string.IsNullOrEmpty(kv[0]) && !string.IsNullOrEmpty(kv[1]))
				.Select(kv =>
					new KeyValuePair<string, string>(Decode(kv[0], isFormData).ToLowerInvariant(),
						Decode(kv[1], isFormData)));
			foreach (var p in pairs)
			{
				dictionary[p.Key] = p.Value;
			}

			return dictionary;
		}

		public static bool IsValidUrl(string url)
		{
			return Uri.TryCreate(url, UriKind.Absolute, out _);
		}
	}
}