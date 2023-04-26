using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnect.VersionShared.Models.DeepLink.Helpers;
using UnityEngine;

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
                var urlOverride = WalletDataHelper.GetOverrideImageUrl(name);
                string uri = null;
                if (urlOverride != null)
                {
                    uri = urlOverride;
                }
                else
                {
                    uri = "https://registry.walletconnect.org/logo/" + size + "/" + id + ".jpeg";
                }

                var texture = await WebHelper.GetTextureFromGenericUri(uri);

                if (texture == null)
                {
                    return;
                }
                
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