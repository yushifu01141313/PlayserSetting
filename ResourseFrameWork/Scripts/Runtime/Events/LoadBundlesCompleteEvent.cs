using System;
using System.Collections.Generic;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadBundlesCompleteEvent
    {
        public readonly Guid RequestID;
        public readonly bool Result;
        public readonly IReadOnlyList<string> ErrorPaths;

        public LoadBundlesCompleteEvent(Guid requestID, bool result, IReadOnlyList<string> errorPaths)
        {
            RequestID = requestID;
            Result = result;
            ErrorPaths = errorPaths;
        }
    }
}