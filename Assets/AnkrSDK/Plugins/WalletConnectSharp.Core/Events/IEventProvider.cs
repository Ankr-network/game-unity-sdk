namespace AnkrSDK.WalletConnectSharp.Core.Events
{
    public interface IEventProvider
    {
        void PropagateEvent(string topic, string responseJson);
    }
}