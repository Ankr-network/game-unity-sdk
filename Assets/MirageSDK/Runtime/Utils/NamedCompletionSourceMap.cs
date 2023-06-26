using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageSDK.Utils
{
	public class NamedCompletionSourceMap
	{
		private readonly Dictionary<string, IUniTaskSource> _completionSourcesDict =
			new Dictionary<string, IUniTaskSource>();

		private readonly Dictionary<string, Type> _operationResultTypeDict = new Dictionary<string, Type>();

		public Type GetOperationResultType(string operationId)
		{
			if (_operationResultTypeDict.TryGetValue(operationId, out var type))
			{
				return type;
			}

			return null;
		}

		public bool HasCompletionSourceFor(string operationId)
		{
			return _completionSourcesDict.ContainsKey(operationId);
		}

		public UniTaskCompletionSource<T> CreateCompletionSource<T>(string operationId)
		{
			if (_completionSourcesDict.ContainsKey(operationId))
			{
				throw new InvalidOperationException($"Completion source for {operationId} already exists");
			}

			var newCompletionSource = new UniTaskCompletionSource<T>();
			_completionSourcesDict.Add(operationId, newCompletionSource);
			_operationResultTypeDict.Add(operationId, typeof(T));
			return newCompletionSource;
		}

		public UniTaskCompletionSource CreateCompletionSource(string operationId)
		{
			if (_completionSourcesDict.ContainsKey(operationId))
			{
				throw new InvalidOperationException($"Completion source for {operationId} already exists");
			}

			var newCompletionSource = new UniTaskCompletionSource();
			_completionSourcesDict.Add(operationId, newCompletionSource);
			return newCompletionSource;
		}

		public void TrySetResult(string name, object result)
		{
			if (_completionSourcesDict.TryGetValue(name, out var completionSource))
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

				_completionSourcesDict.Remove(name);
				_operationResultTypeDict.Remove(name);
			}
			else
			{
				throw new KeyNotFoundException($"Completion source {name} was not found");
			}
		}

		public void TrySetCanceled(string name)
		{
			if (_completionSourcesDict.TryGetValue(name, out var completionSource))
			{
				if (completionSource is ICancelPromise typedCompletionSource)
				{
					typedCompletionSource.TrySetCanceled();
					_completionSourcesDict.Remove(name);
					_operationResultTypeDict.Remove(name);
				}
			}
			else
			{
				throw new KeyNotFoundException($"Completion source {name} was not found");
			}
		}
	}
}