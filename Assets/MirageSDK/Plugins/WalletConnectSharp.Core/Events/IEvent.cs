namespace MirageSDK.WalletConnectSharp.Core.Events
{
    public interface IEvent<in T>
    {
        void SetData(T data);
    }
}