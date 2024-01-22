using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class WarmUpForSceneCompleteEvent
    {
        public readonly Guid RequestID;
        public readonly bool Result;

        public WarmUpForSceneCompleteEvent(Guid requestID, bool result)
        {
            RequestID = requestID;
            Result = result;
        }
    }
}