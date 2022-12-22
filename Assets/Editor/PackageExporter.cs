#if UNITY_EDITOR
using System.IO;
using UnityEditor;

public static class PackageExporter
{
	private const string PackageName = "AnkrSDK";
	private const string ImporterPackageName = "AnkrSDKImporter";

	// The path to the package under the `Assets/` folder.
	private const string PackagePath = "Assets/AnkrSDK";
	private const string ImporterPackagePath = "Assets/AnkrSDKImporter";

	// Path to export to.
	private const string ExportPath = "Build";

	[MenuItem("AnkrSDK/Export Ankr Package")]
	public static void Export()
	{
		ExportPackage($"{ExportPath}/{PackageName}.unitypackage", PackagePath);
	}

	[MenuItem("AnkrSDK/Export Ankr Importer Package")]
	public static void ExportImporter()
	{
		ExportPackage($"{ExportPath}/{ImporterPackageName}.unitypackage", ImporterPackagePath);
	}

	private static void ExportPackage(string exportPath, string packagePath)
	{
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
	}
}
#endif