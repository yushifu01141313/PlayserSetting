using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class UnLoadAllBundlesRequestEvent
    {
        public readonly Guid RequestID;
        public readonly bool UnloadAllObjects;
        public UnLoadAllBundlesRequestEvent(Guid requestID, bool unloadAllObjects)
        {
            RequestID = requestID;
            UnloadAllObjects = unloadAllObjects;
        }
    }
}
