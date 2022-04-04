// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

namespace System.Reactive.Linq.ObservableImpl
{
    internal sealed class AsObservable<TSource>
    {
        private readonly IObservable<TSource> _source;

        public AsObservable(IObservable<TSource> source)
        {
            _source = source;
        }
    }
}
