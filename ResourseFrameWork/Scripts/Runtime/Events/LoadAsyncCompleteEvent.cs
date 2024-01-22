using System;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadAsyncCompleteEvent
    {
        public readonly Guid RequestID;
        public readonly bool Result;
        public readonly string CallbackData;
        public readonly UnityEngine.Object Asset;

        public LoadAsyncCompleteEvent(Guid requestID, bool result, string callbackData, UnityEngine.Object asset)
        {
            RequestID = requestID;
            CallbackData = callbackData;
            Asset = asset;
            Result = result;
        }
    }
}