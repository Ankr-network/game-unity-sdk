using System.Collections.Generic;
using System.IO;
using AnkrSDKImporter.Data;
using AnkrSDKImporter.Editor.Utils;
using SimpleJSON;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace AnkrSDKImporter.Editor
{
	[InitializeOnLoad]
	public class AnkrSDKImporter
	{
		private const string NamePropertyName = "name";
		private const string UrlPropertyName = "url";

		private const string CompanyNameToRejectImport = "Ankr";
		private const string ProductNameToRejectImport = "Ankr SDK";
		private const string ImporterFolderName = "AnkrSDKImporter";
		private const string MetaExt = ".meta";

		static AnkrSDKImporter()
		{
			if (IsOriginProject())
			{
				return;
			}

			PackageManagerUtils.RequestPackagesList(OnPackagesLoaded);
		}

		private static bool IsOriginProject()
		{
			return Application.companyName == CompanyNameToRejectImport
			       && Application.productName == ProductNameToRejectImport;
		}

		[MenuItem("AnkrSDK/Importer/Force Add Packages")]
		public static void ForceAddPackages()
		{
			PackageManagerUtils.RequestPackagesList(OnPackagesLoaded);
		}

		private static void OnPackagesLoaded(bool success, PackageCollection packageCollection)
		{
			if (!success)
			{
				return;
			}

			var settings = Resources.Load<AnkrSDKImporterSettings>("AnkrSDKImporterSettings");

			var filteredPackagesToImport = new List<PackageData>();
			foreach (var packageData in GetPackageData(settings))
			{
				if (packageCollection.Has(packageData))
				{
					Debug.Log($"Skipping package {packageData.PackageName} {packageData.PackageVersionOrUrl} because already exists");
					continue;
				}

				filteredPackagesToImport.Add(packageData);
			}

			AddDataToManifest(filteredPackagesToImport, settings);
		}

		private static List<PackageData> GetPackageData(AnkrSDKImporterSettings settings)
		{
			var packagesToImport = new List<PackageData>();

			foreach (var packageEntry in settings.GetPackageDataEntries())
			{
				packagesToImport.Add(new PackageData(packageEntry));
			}

			return packagesToImport;
		}

		private static void AddDataToManifest(List<PackageData> packagesToImport, AnkrSDKImporterSettings settings)
		{
			var assetsFolderPath = Application.dataPath;
			var assetsDirectoryInfo = new DirectoryInfo(assetsFolderPath);

			if (assetsDirectoryInfo.Parent == null)
			{
				Debug.LogError("AnkrSDKImporter: parent not found for folder " + assetsFolderPath);
				return;
			}

			var packagesFolderPath = Path.Combine(assetsDirectoryInfo.Parent.FullName, "Packages");

			var manifestPath = Path.Combine(packagesFolderPath, "manifest.json");

			if (!File.Exists(manifestPath))
			{
				Debug.LogError("AnkrSDKImporter: package manifest file does not exist");
				return;
			}

			var manifestText = File.ReadAllText(manifestPath);

			if (string.IsNullOrWhiteSpace(manifestText))
			{
				Debug.LogError("AnkrSDKImporter: package manifest file is empty");
				return;
			}

			var jsonParsedObject = JSON.Parse(manifestText);

			const string depdendenciesKey = "dependencies";
			const string scopedRegistriesKey = "scopedRegistries";

			if (!jsonParsedObject.HasKey(depdendenciesKey))
			{
				Debug.LogError("AnkrSDKImporter: Manifest json object does not contain dependencies key");
				return;
			}

			var dependenciesObj = jsonParsedObject[depdendenciesKey];
			foreach (var packageData in packagesToImport)
			{
				dependenciesObj.Add(packageData.PackageName, new JSONString(packageData.PackageVersionOrUrl));
			}

			if (!jsonParsedObject.HasKey(scopedRegistriesKey))
			{
				jsonParsedObject.Add(scopedRegistriesKey, new JSONArray());
			}

			var scopeRegistriesArray = jsonParsedObject[scopedRegistriesKey].AsArray;
			var openUpmRegistryEntryFound = false;
			foreach (var scopeRegistryNode in scopeRegistriesArray.Values)
			{
				var scopeRegistryObject = (JSONObject)scopeRegistryNode;
				var registryName = scopeRegistryObject[NamePropertyName];

				if (registryName is JSONString registryNameString && registryNameString == settings.OpenUpmRegistryName)
				{
					openUpmRegistryEntryFound = true;
					break;
				}
			}

			if (!openUpmRegistryEntryFound)
			{
				scopeRegistriesArray.Add(CreateOpenUpmRegistryObject(settings));
			}

			jsonParsedObject.SetRecursiveInline(false);
			string updatedManifestText = jsonParsedObject.ToString(4);
			File.WriteAllText(manifestPath, updatedManifestText);

			Debug.Log("AnkrSDKImporter: all required packages added to manifest");

			Client.Resolve();

			if (!IsOriginProject())
			{
				Directory.Delete(Path.Combine(Application.dataPath, ImporterFolderName), true);
				File.Delete(Path.Combine(Application.dataPath, ImporterFolderName + MetaExt));
			}
		}

		private static JSONObject CreateOpenUpmRegistryObject(AnkrSDKImporterSettings settings)
		{
			var openUpmJsonObj = new JSONObject();
			openUpmJsonObj.Add(NamePropertyName, new JSONString(settings.OpenUpmRegistryName));
			openUpmJsonObj.Add(UrlPropertyName, new JSONString(settings.OpenUpmRegistryUrl));

			var scopesArray = new JSONArray();
			foreach (var registryScope in settings.RegistryScopes)
			{
				scopesArray.Add(new JSONString(registryScope));
				scopesArray.Add(new JSONString(registryScope));
			}

			openUpmJsonObj.Add("scopes", scopesArray);

			return openUpmJsonObj;
		}
	}
}