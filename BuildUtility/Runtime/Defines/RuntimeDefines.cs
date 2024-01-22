using System.IO;
using UnityEngine;

namespace UGC.PackagerUtility.Runtime.Defines
{
    public static class RuntimeDefines 
    {
        public static readonly string AssetsPath = "Assets";
        
        public static readonly string ResourceFolder = Application.streamingAssetsPath;
        
        public static readonly string BundleManifestName = "BundleManifest.json";

        public static readonly string SceneBlocksMapJsonName = "SceneBlocksMap.json";
        
        public static readonly string ResourceMapJsonName = "ResourceMap.json";
    }
}
