using System.Runtime.InteropServices;

#if UNITY_WEBGL
public class WebGLInterlayer
{
    [DllImport("__Internal")]
    public static extern void SendTransaction(string id, string payload);

    [DllImport("__Internal")]
    public static extern void SignMessage(string id, string payload);
    
    [DllImport("__Internal")]
    public static extern string GetAddresses(string id);
    
    [DllImport("__Internal")]
    public static extern string GetResponses();
}
#endif