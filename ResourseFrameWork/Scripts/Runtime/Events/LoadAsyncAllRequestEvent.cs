using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadAsyncAllRequestEvent
    {
        public Guid RequestID;
        public string ManifestPath;

        public LoadAsyncAllRequestEvent(Guid requestID, string manifestPath)
        {
            ManifestPath = manifestPath;
            RequestID = requestID;
        }
    }
}