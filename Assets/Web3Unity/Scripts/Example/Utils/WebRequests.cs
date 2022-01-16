using System.Text;
using UnityEngine.Networking;

namespace Utility.Utils
{
    public class WebRequests
    {
        public static UnityWebRequest SendJSON(string url, string json) {
            UnityWebRequest requestU= new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            byte[] bytes= GetBytes(json);
            UploadHandlerRaw uH= new UploadHandlerRaw(bytes);
            requestU.uploadHandler= uH;
            requestU.SetRequestHeader("Content-Type", "application/json");
            requestU.downloadHandler = new DownloadHandlerBuffer ();;
            return requestU;
        }
    
        protected static byte[] GetBytes(string str){
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return bytes;
        }

    }
}