using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class UnLoadBundleCompleteEvent
    {
        public readonly Guid RequestID;
        public readonly bool Result;

        public UnLoadBundleCompleteEvent(Guid requestID, bool result)
        {
            RequestID = requestID;
            Result = result;
        }
    }
}