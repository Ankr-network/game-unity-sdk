using System.IO;
using System.Collections.Generic;
using SimpleJSON;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

namespace AnkrSDKImporter.Editor
{
   [InitializeOnLoad]
   public class AnkrSDKImporter
   {
      private const string NamePropertyName = "name";
      private const string UrlPropertyName = "url";
      
      private const string CompanyNameToRejectImport = "Ankr";
      private const string ProductNameToRejectImport = "Ankr SDK";
      
      private const string OpenUpmRegistryName = "package.openupm.com";
      private const string OpenUpmRegistryUrl = "https://package.openupm.com";
      
      private const string UnitaskRegistryScope = "com.cysharp.unitask";
      private const string AnkrSDKRegistryScope = "com.ankr.ankrsdk";

      private static readonly List<PackageData> PackagesToTryToImport = new List<PackageData>()
      {
         new PackageData("com.unity.nuget.newtonsoft-json", "3.0.1", external:false), 
         new PackageData(UnitaskRegistryScope, "2.3.1", external:false), 
         new PackageData(AnkrSDKRegistryScope, "https://github.com/Ankr-network/game-unity-sdk.git?path=Assets/AnkrSDK", external:true)
      };
      
      private static ListRequest _listRequest;
   
      static AnkrSDKImporter()
      {
         if (Application.companyName == CompanyNameToRejectImport
             && Application.productName == ProductNameToRejectImport)
         {
            return;
         }
         
         _listRequest = Client.List();
         EditorApplication.update += EditorApplicationOnUpdate;
      }

      private static void EditorApplicationOnUpdate()
      {
         if (!_listRequest.IsCompleted)
         {
            return;
         }
         
         if (_listRequest.Status == StatusCode.Success)
         {
            var filteredPackagesToImport = new List<PackageData>();
            foreach (var packageData in PackagesToTryToImport)
            {
               if (PackageCollectionContains(_listRequest.Result, packageData))
               {
                  continue;
               }
                  
               filteredPackagesToImport.Add(packageData);
            }
               
            AddDataToManifest(filteredPackagesToImport);
         }
         else
         {
            Debug.Log("AnkrSDKImporter: Could not check for packages: " + _listRequest.Error.message);
         }

         EditorApplication.update -= EditorApplicationOnUpdate;
         _listRequest = null;
      }

      private static bool PackageCollectionContains(PackageCollection collection, PackageData packageData)
      {
         if (packageData.External)
         {
            //if package is external (meaning loaded by URL) we only consider it existing if the package id is already present
            //in the current project package collection
            foreach (var package in collection)
            {
               if (package.packageId == packageData.PackageId)
               {
                  return true;
               }
            }
         }
         else
         {
            //if project is part of unity package registry we need to make sure that the version is the same
            //otherwise we consider it not present in the project and manifest modification will
            //just change the version in the manifest dependencies json object to make sure 
            //versions for all dependencies will be the ones specified in this class
            foreach (var package in collection)
            {
               if (package.packageId == packageData.PackageId && package.version == packageData.PackageVersionOrUrl)
               {
                  return true;
               }
            }
         }

         return false;
      }

      private static void AddDataToManifest(List<PackageData> packagesToImport)
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
            dependenciesObj.Add(packageData.PackageId, new JSONString(packageData.PackageVersionOrUrl));
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
            
            if (registryName is JSONString registryNameString && registryNameString == OpenUpmRegistryName)
            {
               openUpmRegistryEntryFound = true;
               break;
            }
         }

         if (!openUpmRegistryEntryFound)
         {
            scopeRegistriesArray.Add(CreateOpenUpmRegistryObject());
         }

         jsonParsedObject.SetRecursiveInline(false);
         string updatedManifestText = jsonParsedObject.ToString(aIndent:4);
         File.WriteAllText(manifestPath, updatedManifestText);

         Debug.Log("AnkrSDKImporter: all required packages added to manifest");
         
         Client.Resolve();
      }

      private static JSONObject CreateOpenUpmRegistryObject()
      {
         var openUpmJsonObj = new JSONObject();
         openUpmJsonObj.Add(NamePropertyName, new JSONString(OpenUpmRegistryName));
         openUpmJsonObj.Add(UrlPropertyName, new JSONString(OpenUpmRegistryUrl));

         var scopesArray = new JSONArray();
         scopesArray.Add(new JSONString(AnkrSDKRegistryScope));
         scopesArray.Add(new JSONString(UnitaskRegistryScope));
         
         openUpmJsonObj.Add("scopes", scopesArray);

         return openUpmJsonObj;
      }

      [MenuItem("AnkrSDK/Importer/Force Add Packages")]
      public static void ForceAddPackages()
      {
         if (_listRequest == null)
         {
            _listRequest = Client.List();
            EditorApplication.update += EditorApplicationOnUpdate;
         }
         else
         {
            Debug.LogError("AnkrSDKImporter: Package list request is already sent");
         }
      }
   }
}
