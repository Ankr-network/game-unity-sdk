using System;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnect.VersionShared.Models.DeepLink.Helpers;
using UnityEngine;
using UnityEngine.Networking;

namespace MirageSDK.WalletConnect.VersionShared.Models.DeepLink
{
    public class WalletEntry
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

        public bool AllImagesLoaded => LargeIcon != null && MediumIcon != null && SmallIcon != null;

        public async UniTask DownloadImages(string[] sizes = null)
        {
            sizes = sizes ?? new[] { "sm", "md", "lg" };

            foreach (var size in sizes)
            {
                var urlOverride = WalletDataHelper.GetOverrideUrl(name);
                string url = null;
                if (urlOverride != null)
                {
                    url = urlOverride;
                }
                else
                {
                    url = "https://registry.walletconnect.org/logo/" + size + "/" + id + ".jpeg";
                }

                using (var imageRequest = UnityWebRequestTexture.GetTexture(url))
                {
                    try
                    {
                        await imageRequest.SendWebRequest();
                    }
                    catch (UnityWebRequestException e)
                    {
                        var downloadHandler = ((DownloadHandlerTexture)imageRequest.downloadHandler);
                        Debug.LogError($"Exception while loading texture {e.Message} download handler error: {downloadHandler.error}");
                        return;
                    }

#if UNITY_2020_2_OR_NEWER
                    if (imageRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log("Error Getting Wallet Icon: " + imageRequest.error);
                    }
#else
                        if (imageRequest.isHttpError || imageRequest.isNetworkError)
                        {
                            Debug.Log("Error Getting Wallet Icon: " + imageRequest.error);
                        }
#endif
                    else
                    {
                        var downloadHandler = ((DownloadHandlerTexture)imageRequest.downloadHandler);
                        var texture = downloadHandler.texture;
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