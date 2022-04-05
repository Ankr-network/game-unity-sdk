using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AnkrSDK.Core.Utils
{
	/// <summary>
	/// Builder to create correctly formatted url query.
	/// </summary>
	public class UriQueryBuilder
	{
		private const int BuilderLength = 1000;

		private readonly bool _isFormData;
		private bool _hasBaseUri;
		private bool _hasArguments;
		private readonly StringBuilder _stringBuilder;

		public UriQueryBuilder(string baseUri = null, bool isFormData = false)
		{
			_isFormData = isFormData;
			_stringBuilder = new StringBuilder(BuilderLength);
			AppendBaseUri(baseUri);
		}

		/// <summary>
		/// Add baseUrl.
		/// </summary>
		/// <exception cref="InvalidOperationException">baseUrl already added</exception>
		public UriQueryBuilder AppendBaseUri(string baseUri)
		{
			if (_hasBaseUri)
			{
				throw new InvalidOperationException();
			}

			if (!string.IsNullOrEmpty(baseUri))
			{
				_hasBaseUri = true;
				_hasArguments = baseUri.IndexOf('?') != -1;
				_stringBuilder.Append(baseUri);
			}

			return this;
		}

		/// <summary>
		/// Add parameter to query
		/// </summary>
		public UriQueryBuilder AppendArgument(string key, string value)
		{
			if (_hasArguments)
			{
				_stringBuilder.Append('&');
			}
			else
			{
				if (!_hasArguments && _hasBaseUri)
				{
					_stringBuilder.Append('?');
				}

				_hasArguments = true;
			}

			_stringBuilder.Append(UriUtil.Encode(key, _isFormData));
			_stringBuilder.Append('=');
			_stringBuilder.Append(UriUtil.Encode(value, _isFormData));
			return this;
		}

		/// <summary>
		/// Add parameter to query with value.ToString()
		/// </summary>
		public UriQueryBuilder AppendArgument(string key, object value)
		{
			return AppendArgument(key, value.ToString());
		}

		public UriQueryBuilder AppendArgumentIf(bool condition, string name, object value)
		{
			return condition ? AppendArgument(name, value is string ? (string)value : value.ToString()) : this;
		}

		/// <summary>
		/// Builds the query string.
		/// </summary>
		/// <returns>Created query string</returns>
		public string Build()
		{
			return _stringBuilder.ToString();
		}

		/// <summary>
		/// Same as <see cref="Build()"/>
		/// </summary>
		/// <returns>Created query string</returns>
		public override string ToString()
		{
			return Build();
		}

		public static string Build(object request, bool isFormData = false)
		{
			return Build(null, request, isFormData);
		}

		public static string Build(string url, object request, bool isFormData = false)
		{
			switch (request)
			{
				case IDictionary<string, string> pairs:
					return Build(url, pairs, isFormData);
				case KeyValuePair<string, string>[] pairsArray:
					return Build(url, pairsArray, isFormData);
			}

			var builder = new UriQueryBuilder(url, isFormData);
			var fields = ReflectionUtil.GetFields(request);
			foreach (var p in fields)
			{
				var val = p.GetValue(request);
				var typeInfo = p.FieldType.GetTypeInfo();

				if (!typeInfo.IsArray)
				{
					if (TryGetValue(typeInfo, val, out var value))
					{
						builder.AppendArgument(p.Name, value);
					}
				}
				else
				{
					var array = (Array)val;
					foreach (var item in array)
					{
						if (TryGetValue(typeInfo, item, out var value))
						{
							builder.AppendArgument(p.Name, value);
						}
					}
				}
			}

			return  builder.ToString();
		}

		private static bool TryGetValue(TypeInfo type, object value, out string result)
		{
			if (type.IsEnum)
			{
				result = ((int)value).ToString();
				return true;
			}

			if (value != null)
			{
				result = value.ToString();
				return true;
			}

			result = null;
			return false;
		}

		/// <summary>
		///  Build query string from the given dictionary
		/// </summary>
		public static string Build(IDictionary<string, string> request, bool isFormData = false)
		{
			return Build(null, request, isFormData);
		}

		/// <summary>
		///  Build query string from the given KeyValuePair collection.
		/// </summary>
		public static string Build(string url, IEnumerable<KeyValuePair<string, string>> request,
			bool isFormData = false)
		{
			var builder = new UriQueryBuilder(url, isFormData);
			foreach (var p in request)
			{
				builder.AppendArgument(p.Key, p.Value);
			}

			return builder.ToString();
		}
	}
}