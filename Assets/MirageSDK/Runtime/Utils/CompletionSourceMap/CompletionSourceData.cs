using System;
using Cysharp.Threading.Tasks;

namespace MirageSDK.Utils.CompletionSourceMap
{
	public struct CompletionSourceData
	{
		public IUniTaskSource Source;
		public Type ResultType;
	}
}