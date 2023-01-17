using AnkrSDKImporter.Data;

namespace AnkrSDKImporter.Editor
{
    public struct PackageData
    {
        public readonly string PackageId;
        public readonly string PackageVersionOrUrl;
        public readonly bool External;

        public PackageData(PackageDataEntry packageEntry)
        {
            External = packageEntry.External;
            PackageId = packageEntry.PackageName;
            PackageVersionOrUrl = packageEntry.PackageVersionOrUrl;
        }
    }
}