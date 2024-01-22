using System;
using UnityEngine;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class LoadAsyncAllCompleteEvent
    {
        public Guid RequestID;
        public bool IsDone;
        public GameObject Prefab;

        public LoadAsyncAllCompleteEvent(Guid requestID, bool isDone, GameObject prefab)
        {
            RequestID = requestID;
            IsDone = isDone;
            Prefab = prefab;
        }
    }
}
