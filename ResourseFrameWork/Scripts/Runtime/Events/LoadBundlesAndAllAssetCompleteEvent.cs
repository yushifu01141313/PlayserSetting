using System;
using UnityEngine;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadBundlesAndAllAssetCompleteEvent
    {
        public readonly Guid RequestID;
        public readonly bool IsDone;
        public readonly GameObject Prefab;

        public LoadBundlesAndAllAssetCompleteEvent(Guid requestID, bool isDone, GameObject prefab)
        {
            RequestID = requestID;
            IsDone = isDone;
            Prefab = prefab;
        }
    }
}