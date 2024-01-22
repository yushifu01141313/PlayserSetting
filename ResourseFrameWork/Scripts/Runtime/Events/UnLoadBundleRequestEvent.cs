using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class UnLoadBundleRequestEvent
    {
        public readonly Guid RequestID;
        public readonly bool Result;
        public readonly string LoadName;

        public UnLoadBundleRequestEvent(Guid requestID, bool result, string loadName)
        {
            RequestID = requestID;
            Result = result;
            LoadName = loadName;
        }
    }
}