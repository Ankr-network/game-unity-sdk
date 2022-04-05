using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnkrSDK.Core.Utils
{
	internal static class ReflectionUtil
	{
		private static readonly Dictionary<Type, FieldInfo[]> Cache = new Dictionary<Type, FieldInfo[]>();

		private static readonly object SyncObj = new object();

		public static IEnumerable<FieldInfo> GetFields(object obj)
		{
			return GetFields(obj.GetType());
		}

		public static FieldInfo[] GetFields<T>()
		{
			return GetFields(typeof(T));
		}

		public static FieldInfo[] GetFields(Type type)
		{
			lock (SyncObj)
			{
				if (!Cache.TryGetValue(type, out var fields))
				{
					Cache[type] = fields = type.GetRuntimeFields().ToArray();
				}

				return fields;
			}
		}
	}
}