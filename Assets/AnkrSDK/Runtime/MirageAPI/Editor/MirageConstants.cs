using System.IO;

namespace AnkrSDK.MirageAPI.Editor
{
	public static class MirageConstants
	{
		public const string ResourcesPath = "Assets/AnkrSDK/Runtime/MirageAPI/Data/Resources";
		public const string MirageAPISettingsName = "MirageAPISettings";

		public static readonly string DefaultSettingsAssetPath =
			Path.Combine(ResourcesPath, MirageAPISettingsName + ".asset");
	}
}