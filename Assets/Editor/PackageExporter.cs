#if UNITY_EDITOR
using System.IO;
using UnityEditor;

public static class PackageExporter
{
	private const string PackageName = "AnkrSDK";

	// The path to the package under the `Assets/` folder.
	private const string PackagePath = "Assets/AnkrSDK";

	// Path to export to.
	private const string ExportPath = "Build";

	[MenuItem("AnkrSDK/Export Ankr Package")]
	public static void Export()
	{
		ExportPackage($"{ExportPath}/{PackageName}.unitypackage");
	}

	private static void ExportPackage(string exportPath)
	{
		// Ensure export path.
		var dir = new FileInfo(exportPath).Directory;
		if (dir?.Exists == false)
		{
			dir.Create();
		}

		// Export
		AssetDatabase.ExportPackage(
			PackagePath,
			exportPath,
			ExportPackageOptions.Recurse
		);

		EditorUtility.RevealInFinder(exportPath);
	}
}
#endif