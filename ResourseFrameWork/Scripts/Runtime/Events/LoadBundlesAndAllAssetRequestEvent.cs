using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadBundlesAndAllAssetRequestEvent
    {
        public readonly Guid RequestID;
        public readonly string ManifestPath;
        public readonly string BundleFolderPath;

        public LoadBundlesAndAllAssetRequestEvent(Guid requestID, string manifestPath, string bundleFolderPath)
        {
            RequestID = requestID;
            ManifestPath = manifestPath;
            BundleFolderPath = bundleFolderPath;
        }
    }
}