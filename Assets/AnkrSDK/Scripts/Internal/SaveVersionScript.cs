using UnityEditor;
using UnityEngine;

namespace AnkrSDK.Internal
{
	[InitializeOnLoad]
	public class SaveVersionScript
	{
		private const string SdkVersionFieldName = "version";

		static SaveVersionScript()
		{
			var version = PackageInfo.Version;
			PlayerPrefs.SetString(SdkVersionFieldName, version);
		}
	}
}