using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class GetAllManifestPathRequestEvent
    {
        public readonly Guid RequestID;

        public GetAllManifestPathRequestEvent(Guid requestID)
        {
            RequestID = requestID;
        }
    }
}
