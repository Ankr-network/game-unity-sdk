using System;
using System.Collections.Generic;

namespace AnkrSDKImporter.Editor
{
    [Serializable]
    public class ScopedRegistry
    {
        public string name;
        public string url;
        public List<string> scopes;
    }
    
    [Serializable]
    public class ManifestDTO
    {
        public Dictionary<string, string> dependencies;
        public List<ScopedRegistry> scopedRegistries;
    }
}