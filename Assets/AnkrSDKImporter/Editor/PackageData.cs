using AnkrSDKImporter.Data;

namespace AnkrSDKImporter.Editor
{
    public struct PackageData
    {
        public readonly string PackageId;
        public readonly string PackageVersionOrUrl;
        public readonly bool External;

        public PackageData(string packageId, string packageVersionOrUrl, bool external)
        {
            External = external;
            PackageId = packageId;
            PackageVersionOrUrl = packageVersionOrUrl;
        }

        public PackageData(PackageDataEntry packageEntry)
        {
            External = packageEntry.External;
            PackageId = packageEntry.PackageId;
            PackageVersionOrUrl = packageEntry.PackageVersionOrUrl;
        }
    }
}