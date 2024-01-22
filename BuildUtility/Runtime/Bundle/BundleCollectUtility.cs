#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

namespace UGC.PackagerUtility.Runtime.Bundle
{
    public static class BundleCollectUtility
    {
        public static bool GetBundleContent(Dictionary<HashSet<string>, HashSet<string>> assetSceneMap, string bundleNamePattern,
            out IBundleBuildContent bundleBuildContent, out Dictionary<string, HashSet<string>> bundleSceneMap)
        {
            if (!IsAssetSceneMapValid(assetSceneMap))
            {
                Debug.LogErrorFormat("Fail to get bundle content, contain invalid {0}", nameof(assetSceneMap));
                bundleBuildContent = null;
                bundleSceneMap = null;
                return false;
            }

            AssetBundleBuild[] bundleLayout = GetBundleLayout(assetSceneMap, bundleNamePattern, out bundleSceneMap);
            bundleBuildContent = new BundleBuildContent(bundleLayout);

            return true;
        }
        
        public static bool GetBundleContent(SortedDictionary<string,string> resourceHashBundleMap,
            out IBundleBuildContent bundleBuildContent,out List<string> bundleNames)
        {
            if (!IsAssetSceneMapValid(resourceHashBundleMap))
            {
                Debug.LogErrorFormat("Fail to get bundle content, contain invalid {0}", nameof(resourceHashBundleMap));
                bundleBuildContent = null;
                bundleNames = null;
                return false;
            }

            foreach (var VARIABLE in resourceHashBundleMap)
            {
                Debug.Log(VARIABLE.Key+" : "+VARIABLE.Value);
            }
            AssetBundleBuild[] bundleLayout = GetBundleLayout(resourceHashBundleMap,out bundleNames);
            bundleBuildContent = new BundleBuildContent(bundleLayout);
            return true;
        }
        
        private static AssetBundleBuild[] GetBundleLayout(SortedDictionary<string,string> hashDependencyMap,out List<string> bundleNames)
        {
            AssetBundleBuild[] build = new AssetBundleBuild[hashDependencyMap.Count];
            bundleNames = new List<string>();
            int index = 0;
            foreach (var hashDependencyItem in hashDependencyMap)
            {
                build[index].assetNames = new string[]{hashDependencyItem.Value};
                build[index].assetBundleName = hashDependencyItem.Key;
                bundleNames.Add(hashDependencyItem.Key);
                index++;
            }
            return build;
        }

        private static AssetBundleBuild[] GetBundleLayout(Dictionary<HashSet<string>, HashSet<string>> assetSceneMap
            , string bundleNamePattern, out Dictionary<string, HashSet<string>> bundleSceneMap)
        {
            List<AssetBundleBuild> bundleLayout = new List<AssetBundleBuild>(assetSceneMap.Count);
            bundleSceneMap = new Dictionary<string, HashSet<string>>(assetSceneMap.Count);

            int index = 0;
            foreach (var assetSceneItem in assetSceneMap)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetNames = assetSceneItem.Key.ToArray();
                build.assetBundleName = string.Format("{0}{1}", bundleNamePattern, index++);
                bundleLayout.Add(build);

                bundleSceneMap.Add(build.assetBundleName, assetSceneItem.Value);
            }
            return bundleLayout.ToArray();
        }

        private static bool IsAssetSceneMapValid(Dictionary<HashSet<string>, HashSet<string>> assetSceneMap)
        {
            if (assetSceneMap == null || assetSceneMap.Count == 0)
            {
                return false;
            }
            foreach (var assetSceneItem in assetSceneMap)
            {
                if (assetSceneItem.Key == null ||
                    assetSceneItem.Key.Count == 0 ||
                    assetSceneItem.Value == null ||
                    assetSceneItem.Value.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }
        
        private static bool IsAssetSceneMapValid(SortedDictionary<string,string> resourceHashBundleMap)
        {
            if (resourceHashBundleMap == null || resourceHashBundleMap.Count == 0)
            {
                return false;
            }
            foreach (var assetSceneItem in resourceHashBundleMap)
            {
                if (assetSceneItem.Key == null ||
                    assetSceneItem.Value == null)
                {
                    Debug.LogWarning("Not find dependency.");
                }
            }
            return true;
        }
    }
}
#endif
