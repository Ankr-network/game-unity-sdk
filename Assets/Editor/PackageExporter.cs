#if UNITY_EDITOR
using System.IO;
using MirageSDKImporter.Data;
using MirageSDKImporter.Editor;
using MirageSDKImporter.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	public static class PackageExporter
	{
		private const string ImporterPackageName = "MirageSDKImporter";

		// The path to the package under the `Assets/` folder.
		private const string ImporterPackagePath = "Assets/MirageSDKImporter";

		// Path to export to.
		private const string ExportPath = "Build";

		private static readonly string[] PackagesToUpdateVersion =
		{
			"com.unity.nuget.newtonsoft-json", "com.cysharp.unitask"
		};

		[MenuItem("MirageSDK/Export Mirage Importer Package")]
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

				var settings = Resources.Load<MirageSDKImporterSettings>("MirageSDKImporterSettings");
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
}
#endif