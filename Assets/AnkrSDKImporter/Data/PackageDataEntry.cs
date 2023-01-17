using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnkrSDKImporter.Data
{
	[Serializable]
	public class PackageDataEntry
	{
		[FormerlySerializedAs("PackageId")]
		public string PackageName;
		[Tooltip("Will be filled for internal packages during SDK package creation")]
		public string PackageVersionOrUrl;
		public bool External;
	}
}