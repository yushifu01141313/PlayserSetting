using System;
using System.Collections.Generic;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class MultipleLoadAsyncRequestEvent
    {
        public readonly Guid RequestID;
        public readonly ManifestTypes ManifestType;
        public readonly List<string> LoadNames;

        public MultipleLoadAsyncRequestEvent(Guid requestID, ManifestTypes manifestType, List<string> loadNames)
        {
            RequestID = requestID;
            ManifestType = manifestType;
            LoadNames = loadNames;
        }
    }
}
