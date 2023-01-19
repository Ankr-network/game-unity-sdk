#if UNITY_EDITOR
using System.IO;
using AnkrSDKImporter.Data;
using AnkrSDKImporter.Editor;
using AnkrSDKImporter.Editor.Utils;
using UnityEditor;
using UnityEngine;

public static class PackageExporter
{
	private const string ImporterPackageName = "AnkrSDKImporter";

	// The path to the package under the `Assets/` folder.
	private const string ImporterPackagePath = "Assets/AnkrSDKImporter";

	// Path to export to.
	private const string ExportPath = "Build";

	private static readonly string[] PackagesToUpdateVersion =
	{
		"com.unity.nuget.newtonsoft-json", "com.cysharp.unitask"
	};

	[MenuItem("AnkrSDK/Export Ankr Importer Package")]
	public static void ExportImporter()
	{
		ExportPackage($"{ExportPath}/{ImporterPackageName}.unitypackage", ImporterPackagePath);
	}

	private static void ExportPackage(string exportPath, string packagePath)
	{
		PackageManagerUtils.RequestPackagesList((success, packagesCollection) =>
		{
			if (!success)
			{
				return;
			}

			var settings = Resources.Load<AnkrSDKImporterSettings>("AnkrSDKImporterSettings");
			foreach (var packageName in PackagesToUpdateVersion)
			{
				var version = packagesCollection.FindVersionFor(packageName);
				if (version != null)
				{
					settings.SetVersion(packageName, version);
				}
			}

			EditorUtility.SetDirty(settings);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			// Ensure export path.
			var dir = new FileInfo(exportPath).Directory;
			if (dir?.Exists == false)
			{
				dir.Create();
			}

			// Export
			AssetDatabase.ExportPackage(
				packagePath,
				exportPath,
				ExportPackageOptions.Recurse
			);

			EditorUtility.RevealInFinder(exportPath);
		});
	}
}
#endif