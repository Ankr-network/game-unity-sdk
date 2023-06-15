using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace MirageSDK.Utils
{
	public class NamedCompletionSourceMap
	{
		private readonly Dictionary<string, IUniTaskSource> _completionSourcesDict =
			new Dictionary<string, IUniTaskSource>();

		public bool HasCompletionSourceFor(string name)
		{
			return _completionSourcesDict.ContainsKey(name);
		}

		public UniTaskCompletionSource<T> CreateCompletionSource<T>(string name)
		{
			if (_completionSourcesDict.ContainsKey(name))
			{
				throw new InvalidOperationException($"Completion source for {name} already exists");
			}

			var newCompletionSource = new UniTaskCompletionSource<T>();
			_completionSourcesDict.Add(name, newCompletionSource);
			return newCompletionSource;
		}

		public UniTaskCompletionSource CreateCompletionSource(string name)
		{
			if (_completionSourcesDict.ContainsKey(name))
			{
				throw new InvalidOperationException($"Completion source for {name} already exists");
			}

			var newCompletionSource = new UniTaskCompletionSource();
			_completionSourcesDict.Add(name, newCompletionSource);
			return newCompletionSource;
		}

		public void TrySetResult<T>(string name, T result)
		{
			if (_completionSourcesDict.TryGetValue(name, out var completionSource))
			{
				if (completionSource is UniTaskCompletionSource<T> typedCompletionSource)
				{
					typedCompletionSource.TrySetResult(result);
					_completionSourcesDict.Remove(name);
				}
				else
				{
					throw new InvalidCastException($"Completion source {name} does not hold type {typeof(T).Name}");
				}
			}
			else
			{
				throw new KeyNotFoundException($"Completion source {name} was not found");
			}
		}

		public void TrySetResult(string name)
		{
			if (_completionSourcesDict.TryGetValue(name, out var completionSource))
			{
				if (completionSource is UniTaskCompletionSource typedCompletionSource)
				{
					typedCompletionSource.TrySetResult();
					_completionSourcesDict.Remove(name);
				}
				else
				{
					throw new InvalidCastException($"Completion source {name} is not typeless");
				}
			}
			else
			{
				throw new KeyNotFoundException($"Completion source {name} was not found");
			}
		}

		public void TrySetCanceled<T>(string name)
		{
			if (_completionSourcesDict.TryGetValue(name, out var completionSource))
			{
				if (completionSource is UniTaskCompletionSource<T> typedCompletionSource)
				{
					typedCompletionSource.TrySetCanceled();
					_completionSourcesDict.Remove(name);
				}
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
				if (completionSource is UniTaskCompletionSource typedCompletionSource)
				{
					typedCompletionSource.TrySetCanceled();
					_completionSourcesDict.Remove(name);
				}
			}
			else
			{
				throw new KeyNotFoundException($"Completion source {name} was not found");
			}
		}
	}
}