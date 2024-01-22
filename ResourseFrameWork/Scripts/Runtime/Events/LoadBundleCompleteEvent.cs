using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadBundleCompleteEvent
    {
        public readonly Guid RequestID;
        public readonly bool Result;
        public readonly string[] ErrorPath;

        public LoadBundleCompleteEvent(Guid requestID, bool result, string[] errorPath)
        {
            RequestID = requestID;
            Result = result;
            ErrorPath = errorPath;
        }
    }
}
