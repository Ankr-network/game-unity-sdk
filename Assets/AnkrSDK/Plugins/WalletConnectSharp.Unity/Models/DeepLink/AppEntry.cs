using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink
{
    public class AppEntry
    {
        public string id;
        public string name;
        public string homepage;
        public string[] chains;
        public AppInfo app;
        public MobileAppData mobile;
        public MobileAppData desktop;
        public AppMetadata metadata;

        public Sprite LargeIcon;
        public Sprite MediumIcon;
        public Sprite SmallIcon;

        public async UniTask DownloadImages(string[] sizes = null)
        {
            sizes = sizes ?? new[] { "sm", "md", "lg" };

            foreach (var size in sizes)
            {
                var url = "https://registry.walletconnect.org/logo/" + size + "/" + id + ".jpeg";

                using (var imageRequest = UnityWebRequestTexture.GetTexture(url))
                {
                    await imageRequest.SendWebRequest();

                    if (imageRequest.isHttpError || imageRequest.isNetworkError)
                    {
                        Debug.Log("Error Getting Wallet Icon: " + imageRequest.error);
                    }
                    else
                    {
                        var texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
                        var sprite = Sprite.Create(texture,
                            new Rect(0.0f, 0.0f, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f), 100.0f);

                        switch (size)
                        {
                            case "sm":
                                SmallIcon = sprite;
                                break;
                            case "md":
                                MediumIcon = sprite;
                                break;
                            case "lg":
                                LargeIcon = sprite;
                                break;
                        }
                    }
                }
            }
        }

    }
}