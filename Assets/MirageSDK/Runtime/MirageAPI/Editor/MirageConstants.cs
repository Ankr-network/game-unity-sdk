using System.IO;

namespace MirageSDK.MirageAPI.Editor
{
	public static class MirageConstants
	{
		public const string ResourcesPath = "Assets/MirageSDK/Runtime/MirageAPI/Data/Resources";
		public const string MirageAPISettingsName = "MirageAPISettings";

		public static readonly string DefaultSettingsAssetPath =
			Path.Combine(ResourcesPath, MirageAPISettingsName + ".asset");
	}
}