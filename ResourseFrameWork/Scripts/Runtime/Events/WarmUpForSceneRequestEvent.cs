using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class WarmUpForSceneRequestEvent
    {
        public readonly Guid RequestID;
        public readonly string LoadName;

        public WarmUpForSceneRequestEvent(Guid requestID, string loadName)
        {
            RequestID = requestID;
            LoadName = loadName;
        }
    }
}