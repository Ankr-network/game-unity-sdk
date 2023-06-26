using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageSDK.Utils.CompletionSourceMap
{
	public class CompletionSourceMap
	{
		private readonly Dictionary<string, CompletionSourceData> _completionSourceDataDict =
			new Dictionary<string, CompletionSourceData>();

		public Type GetOperationResultType(string operationId)
		{
			if (_completionSourceDataDict.TryGetValue(operationId, out var data))
			{
				return data.ResultType;
			}

			return null;
		}

		public bool HasCompletionSourceFor(string operationId)
		{
			return _completionSourceDataDict.ContainsKey(operationId);
		}

		public UniTaskCompletionSource<T> CreateCompletionSource<T>(string operationId)
		{
			if (_completionSourceDataDict.ContainsKey(operationId))
			{
				throw new InvalidOperationException($"Completion source for {operationId} already exists");
			}

			var newCompletionSource = new UniTaskCompletionSource<T>();
			_completionSourceDataDict.Add(operationId, new CompletionSourceData
			{
				Source = newCompletionSource,
				ResultType = typeof(T)
			});
			return newCompletionSource;
		}

		public UniTaskCompletionSource CreateCompletionSource(string operationId)
		{
			if (_completionSourceDataDict.ContainsKey(operationId))
			{
				throw new InvalidOperationException($"Completion source for {operationId} already exists");
			}

			var newCompletionSource = new UniTaskCompletionSource();
			_completionSourceDataDict.Add(operationId, new CompletionSourceData
			{
				Source = newCompletionSource
			});
			return newCompletionSource;
		}

		public void TrySetResult(string name, object result)
		{
			if (_completionSourceDataDict.TryGetValue(name, out var completionSource))
			{
				const string typeSetResultMethodName = "TrySetResult";
				var completionSourceType = completionSource.GetType();
				var trySetResultMethod = completionSourceType.GetMethod(typeSetResultMethodName);
				if (trySetResultMethod == null)
				{
					Debug.LogError($"TrySetResult method not found in {completionSourceType.Name}");
				}
				else
				{
					var parameters = result == null ? null : new [] { result };
					trySetResultMethod.Invoke(completionSource, parameters);
				}

				_completionSourceDataDict.Remove(name);
			}
			else
			{
				throw new KeyNotFoundException($"Completion source {name} was not found");
			}
		}

		public void TrySetCanceled(string name)
		{
			if (_completionSourceDataDict.TryGetValue(name, out var completionSource))
			{
				if (completionSource.Source is ICancelPromise typedCompletionSource)
				{
					typedCompletionSource.TrySetCanceled();
					_completionSourceDataDict.Remove(name);
				}
			}
			else
			{
				throw new KeyNotFoundException($"Completion source {name} was not found");
			}
		}
	}
}