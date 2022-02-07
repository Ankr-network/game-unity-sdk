#if UNITY_EDITOR
using System.IO;
using UnityEditor;

public static class PackageExporter
{
	private const string PackageName = "MirageSDK";

	// The path to the package under the `Assets/` folder.
	private const string PackagePath = "Assets/MirageSDK";

	// Path to export to.
	private const string ExportPath = "Build";
	
	[MenuItem("MirageSDK/Export Mirage Package")]
	public static void Export()
	{
		ExportPackage($"{ExportPath}/{PackageName}.unitypackage");
	}

	private static void ExportPackage(string exportPath)
	{
		// Ensure export path.
		var dir = new FileInfo(exportPath).Directory;
		if (dir is { Exists: false })
		{
			dir.Create();
		}

		// Export
		AssetDatabase.ExportPackage(
			PackagePath,
			exportPath,
			ExportPackageOptions.Recurse
		);
	}
}
#endif