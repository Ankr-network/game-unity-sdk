using MirageSDKImporter.Data;

namespace MirageSDKImporter.Editor
{
	public struct PackageData
	{
		public readonly string PackageName;
		public readonly string PackageVersionOrUrl;
		public readonly bool External;

		public PackageData(PackageDataEntry packageEntry)
		{
			External = packageEntry.External;
			PackageName = packageEntry.PackageName;
			PackageVersionOrUrl = packageEntry.PackageVersionOrUrl;
		}
	}
}