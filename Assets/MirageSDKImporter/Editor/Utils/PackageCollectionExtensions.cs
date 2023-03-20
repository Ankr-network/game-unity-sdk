using System;
using UnityEditor.PackageManager;
using UnityEngine;

namespace MirageSDKImporter.Editor.Utils
{
	public static class PackageCollectionExtensions
	{
		public static string FindVersionFor(this PackageCollection collection, string name)
		{
			foreach (var package in collection)
			{
				if (package.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return package.version;
				}
			}

			Debug.LogError($"{name} not found in the collection");
			return null;
		}

		public static bool Has(this PackageCollection collection, PackageData packageData)
		{
			if (packageData.External)
			{
				//if package is external (meaning loaded by URL) we only consider it existing if the package id is already present
				//in the current project package collection
				foreach (var package in collection)
				{
					if (package.name.Equals(packageData.PackageName, StringComparison.InvariantCultureIgnoreCase))
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
					if (package.name.Equals(packageData.PackageName, StringComparison.InvariantCultureIgnoreCase)
					    && package.version == packageData.PackageVersionOrUrl)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}