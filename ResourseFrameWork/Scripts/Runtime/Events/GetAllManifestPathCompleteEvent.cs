using System;
using System.Collections.Generic;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class GetAllManifestPathCompleteEvent
    {
        public readonly Guid RequestID;
        public readonly bool Result;
        public readonly List<string> AllManifestMap;

        public GetAllManifestPathCompleteEvent(Guid requestID, bool result, List<string> allManifestMap)
        {
            RequestID = requestID;
            Result = result;
            AllManifestMap = allManifestMap;
        }
    }
}
