namespace AnkrSDKImporter.Editor
{
    public struct PackageData
    {
        public readonly string PackageId;
        public readonly string PackageVersionOrUrl;

        public PackageData(string packageId, string packageVersionOrUrl)
        {
            PackageId = packageId;
            PackageVersionOrUrl = packageVersionOrUrl;
        }
    }
}