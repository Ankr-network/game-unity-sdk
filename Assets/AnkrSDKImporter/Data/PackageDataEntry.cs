using System;
using UnityEngine;

namespace AnkrSDKImporter.Data
{
	[Serializable]
	public class PackageDataEntry
	{
		public string PackageId;
		[Tooltip("Will be filled for internal packages during SDK package creation")]
		public string PackageVersionOrUrl;
		public bool External;
	}
}