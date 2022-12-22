using System.IO;
using System.Collections.Generic;
using System.Linq;
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
      private const string CompanyNameToRejectImport = "Ankr";
      private const string ProductNameToRejectImport = "Ankr SDK";

      private static readonly List<PackageData> PackagesToTryToImport = new List<PackageData>()
      {
         new PackageData("com.cysharp.unitask", "2.3.1"), 
         new PackageData("com.unity.nuget.newtonsoft-json", "3.0.2"), 
         new PackageData("com.ankr.ankrsdk", "https://github.com/Ankr-network/game-unity-sdk.git?path=Assets/AnkrSDK")
      };

      private const string UnitaskRegistryScope = "com.cysharp.unitask";
      private const string AnkrSDKRegistryScope = "com.ankr.ankrsdk";
      
      private const string OpenUpmRegistryName = "package.openupm.com";
      private const string OpenUpmRegistryUrl = "https://package.openupm.com";
      
      private static ListRequest _listRequest;
   
      static AnkrSDKImporter()
      {
         if (Application.companyName == CompanyNameToRejectImport
             && Application.productName == ProductNameToRejectImport) 
            return;
         
         _listRequest = Client.List();
         EditorApplication.update += EditorApplicationOnUpdate;
      }

      private static void EditorApplicationOnUpdate()
      {
         if (_listRequest.IsCompleted)
         {
            if (_listRequest.Status == StatusCode.Success)
            {
               var filteredPackagesToImport = new List<PackageData>();
               foreach (PackageData packageData in PackagesToTryToImport)
               {
                  if (PackageCollectionContains(_listRequest.Result, packageData.PackageId))
                     continue;
                  
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
      }

      private static bool PackageCollectionContains(PackageCollection collection, string packageId)
      {
         return collection.Any(package => package.name == packageId);
      }

      private static void AddDataToManifest(List<PackageData> packagesToImport)
      {
         string assetsFolderPath = Application.dataPath;
         string packagesFolderPath = assetsFolderPath.TrimEnd(new [] { '/'})
            .TrimEnd(new [] { '\\'})
            .TrimEnd("Assets".ToCharArray()) + "Packages";

         string manifestPath = Path.Combine(packagesFolderPath, "manifest.json");

         if (!File.Exists(manifestPath))
         {
            Debug.LogError("AnkrSDKImporter: package manifest file does not exist");
            return;
         }
         
         string manifestText = File.ReadAllText(manifestPath);

         if (string.IsNullOrWhiteSpace(manifestText))
         {
            Debug.LogError("AnkrSDKImporter: package manifest file is empty");
            return;
         }
         
         JSONNode jsonParsedObject = JSON.Parse(manifestText);

         const string depdendenciesKey = "dependencies";
         const string scopedRegistriesKey = "scopedRegistries";
         
         if (!jsonParsedObject.HasKey(depdendenciesKey))
         {
            Debug.LogError("AnkrSDKImporter: Manifest json object does not contain dependencies key");
            return;
         }
         
         JSONNode dependenciesObj = jsonParsedObject[depdendenciesKey];
         foreach (PackageData packageData in packagesToImport)
         {
            dependenciesObj.Add(packageData.PackageId, new JSONString(packageData.PackageVersionOrUrl));
         }

         if (!jsonParsedObject.HasKey(scopedRegistriesKey))
         {
            jsonParsedObject.Add(scopedRegistriesKey, new JSONArray());
         }

         JSONArray scopeRegistriesArray = jsonParsedObject[scopedRegistriesKey].AsArray;
         bool openUpmRegistryEntryFound = false;
         foreach (JSONNode scopeRegistryNode in scopeRegistriesArray.Values)
         {
            var scopeRegistryObject = (JSONObject)scopeRegistryNode;
            JSONNode registryName = scopeRegistryObject["name"];
            
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
         openUpmJsonObj.Add("name", new JSONString(OpenUpmRegistryName));
         openUpmJsonObj.Add("url", new JSONString(OpenUpmRegistryUrl));

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
