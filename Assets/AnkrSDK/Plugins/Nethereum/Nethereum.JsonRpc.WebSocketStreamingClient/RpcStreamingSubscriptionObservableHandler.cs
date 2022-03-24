using Nethereum.JsonRpc.Client;
using System;
using Cysharp.Threading.Tasks.Internal;
using Nethereum.JsonRpc.Client.Streaming;

namespace Nethereum.JsonRpc.WebSocketStreamingClient
{

    public class RpcStreamingSubscriptionObservableHandler<TSubscriptionDataResponse> : RpcStreamingSubscriptionHandler<TSubscriptionDataResponse>
    {
        protected AsyncSubject<string> SubscribeResponseSubject { get; set; }
        protected AsyncSubject<bool> UnsubscribeResponseSubject { get; set; }
        protected AsyncSubject<TSubscriptionDataResponse> SubscriptionDataResponseSubject { get; set; }
        
        protected RpcStreamingSubscriptionObservableHandler(IStreamingClient streamingClient, IUnsubscribeSubscriptionRpcRequestBuilder unsubscribeSubscriptionRpcRequestBuilder):base(streamingClient, unsubscribeSubscriptionRpcRequestBuilder)
        {
            SubscribeResponseSubject = new AsyncSubject<string>();
            UnsubscribeResponseSubject = new AsyncSubject<bool>();
            SubscriptionDataResponseSubject = new AsyncSubject<TSubscriptionDataResponse>();
        }

        public IObservable<string> GetSubscribeResponseAsObservable()
        {
            return SubscribeResponseSubject;
        }

        public IObservable<TSubscriptionDataResponse> GetSubscriptionDataResponsesAsObservable()
        {
            return SubscriptionDataResponseSubject;
        }

        public IObservable<bool> GetUnsubscribeResponseAsObservable()
        {
            return UnsubscribeResponseSubject;
        }

        protected override void HandleSubscribeResponseError(RpcResponseException exception)
        {
            SubscribeResponseSubject.OnError(exception);
        }

        protected override void HandleUnsubscribeResponseError(RpcResponseException exception)
        {
            UnsubscribeResponseSubject.OnError(exception);
        }

        protected override void HandleDataResponseError(RpcResponseException exception)
        {
            SubscriptionDataResponseSubject.OnError(exception);
        }

        protected override void HandleSubscribeResponse(string subscriptionId)
        {
            SubscribeResponseSubject.OnNext(subscriptionId);
            SubscribeResponseSubject.OnCompleted();
        }

        protected override void HandleUnsubscribeResponse(bool success)
        {
            UnsubscribeResponseSubject.OnNext(success);
            UnsubscribeResponseSubject.OnCompleted();
            SubscriptionDataResponseSubject.OnCompleted();
        }

        protected override void HandleDataResponse(TSubscriptionDataResponse subscriptionDataResponse)
        {
            SubscriptionDataResponseSubject.OnNext(subscriptionDataResponse);
        }
    }
}
