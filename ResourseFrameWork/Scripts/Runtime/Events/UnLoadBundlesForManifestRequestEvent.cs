using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class UnLoadBundlesForManifestRequestEvent
    {
        public readonly Guid RequestID;
        public readonly string UnLoadManifestPath;
        public UnLoadBundlesForManifestRequestEvent(Guid requestID, string unLoadManifestPath)
        {
            RequestID = requestID;
            UnLoadManifestPath = unLoadManifestPath;
        }
    }
}
