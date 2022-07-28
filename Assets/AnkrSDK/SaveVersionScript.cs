using UnityEditor;

namespace AnkrSDK.Stats
{
	[InitializeOnLoad]
	public class SaveVersionScript
	{
		private const string SdkVersionFieldName = "version";

		static SaveVersionScript()
		{
			var version = PackageInfo.Version;
			EditorPrefs.SetString(SdkVersionFieldName, version);
		}
	}
}