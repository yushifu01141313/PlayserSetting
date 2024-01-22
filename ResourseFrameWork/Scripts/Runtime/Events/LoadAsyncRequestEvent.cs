using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadAsyncRequestEvent
    {
        public readonly Guid RequestID;
        public readonly ManifestTypes ManifestType;
        public readonly string LoadName;

        public LoadAsyncRequestEvent(Guid requestID, ManifestTypes manifestType, string loadName)
        {
            RequestID = requestID;
            ManifestType = manifestType;
            LoadName = loadName;
        }
    }
}