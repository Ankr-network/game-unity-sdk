using Nethereum.JsonRpc.Client;
using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Internal;
using Nethereum.JsonRpc.Client.Streaming;

namespace Nethereum.JsonRpc.WebSocketStreamingClient
{
    public class RpcStreamingResponseObservableHandler<TResponse> : RpcStreamingRequestResponseHandler<TResponse>
    {
        protected AsyncSubject<TResponse> ResponseSubject { get; set; }

        protected RpcStreamingResponseObservableHandler(IStreamingClient streamingClient):base(streamingClient)
        {
            ResponseSubject = new AsyncSubject<TResponse>();
        }

        public IObservable<TResponse> GetResponseAsObservable()
        {
            return ResponseSubject;
        }

        protected override void HandleResponse(TResponse subscriptionDataResponse)
        {
            ResponseSubject.OnNext(subscriptionDataResponse);
            ResponseSubject.OnCompleted();
        }

        protected override void HandleResponseError(RpcResponseException exception)
        {
            ResponseSubject.OnError(exception);
        }
    }
}
