using System;
using System.Reactive.Subjects;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.Streaming;

namespace Nethereum.RPC.Reactive.RpcStreaming
{

    public class RpcStreamingSubscriptionObservableHandler<TSubscriptionDataResponse> : RpcStreamingSubscriptionHandler<TSubscriptionDataResponse>
    {
        protected Subject<string> SubscribeResponseSubject { get; set; }
        protected Subject<bool> UnsubscribeResponseSubject { get; set; }
        protected Subject<TSubscriptionDataResponse> SubscriptionDataResponseSubject { get; set; }
        
        protected RpcStreamingSubscriptionObservableHandler(IStreamingClient streamingClient, IUnsubscribeSubscriptionRpcRequestBuilder unsubscribeSubscriptionRpcRequestBuilder):base(streamingClient, unsubscribeSubscriptionRpcRequestBuilder)
        {
            SubscribeResponseSubject = new Subject<string>();
            UnsubscribeResponseSubject = new Subject<bool>();
            SubscriptionDataResponseSubject = new Subject<TSubscriptionDataResponse>();
        }

        public IDisposable SetSubscribeResponseAsObservable(IObserver<string> observer)
        {
            return SubscribeResponseSubject.Subscribe(observer);
        }

        public IDisposable SetSubscriptionDataResponsesAsObservable(IObserver<TSubscriptionDataResponse> observer)
        {
            return SubscriptionDataResponseSubject.Subscribe(observer);
        }

        public IDisposable GetUnsubscribeResponseAsObservable(IObserver<bool> observer)
        {
            return UnsubscribeResponseSubject.Subscribe(observer);
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