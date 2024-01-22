using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadBundleRequestEvent
    {
        public readonly Guid RequestID;
        public readonly string[] ManifestPaths;
        public readonly ManifestTypes ManifestType;
        public readonly string BundleFolderPath;

        public LoadBundleRequestEvent(Guid requestID, string[] manifestPaths, ManifestTypes manifestType,
            string bundleFolderPath)
        {
            RequestID = requestID;
            ManifestPaths = manifestPaths;
            ManifestType = manifestType;
            BundleFolderPath = bundleFolderPath;
        }
    }
}