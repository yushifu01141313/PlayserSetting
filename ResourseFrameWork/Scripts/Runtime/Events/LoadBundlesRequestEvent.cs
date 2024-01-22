using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadBundlesRequestEvent
    {
        public readonly Guid RequestID;
        public readonly string ManifestPath;
        public readonly ManifestTypes ManifestType;
        public readonly string BundleFolderPath;

        public LoadBundlesRequestEvent(Guid requestID, string manifestPath, ManifestTypes manifestType,
            string bundleFolderPath)
        {
            RequestID = requestID;
            ManifestPath = manifestPath;
            ManifestType = manifestType;
            BundleFolderPath = bundleFolderPath;
        }
    }
}