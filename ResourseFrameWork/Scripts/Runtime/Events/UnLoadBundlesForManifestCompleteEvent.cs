using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class UnLoadBundlesForManifestCompleteEvent
    {
        public readonly Guid RequestID;
        public readonly bool Result;
        public UnLoadBundlesForManifestCompleteEvent(Guid requestID, bool result)
        {
            RequestID = requestID;
            Result = result;
        }
    }
}
