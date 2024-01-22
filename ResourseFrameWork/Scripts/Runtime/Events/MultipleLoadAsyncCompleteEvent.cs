using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGC.ResourceFramework.Runtime.Events
{
    public class MultipleLoadAsyncCompleteEvent
    {
        public readonly Guid RequestID;
        public readonly bool Result;
        public readonly List<LoadAssetData> LoadAssetDatas;
        public readonly List<LoadAssetData> LoadAssetErrorDatas;

        public MultipleLoadAsyncCompleteEvent(Guid requestID, bool result, List<LoadAssetData> loadAssetDatas, List<LoadAssetData> loadAssetErrorDatas)
        {
            RequestID = requestID;
            Result = result;
            LoadAssetDatas = loadAssetDatas;
            LoadAssetErrorDatas = loadAssetErrorDatas;
        }
    }

    public class LoadAssetData
    {
        public readonly string LoadName;
        public readonly UnityEngine.Object Asset;

        public LoadAssetData(string loadName, UnityEngine.Object asset)
        {
            LoadName = loadName;
            Asset = asset;
        }
    }
}
