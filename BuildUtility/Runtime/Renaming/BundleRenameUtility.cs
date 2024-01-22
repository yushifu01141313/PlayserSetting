using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace UGC.PackagerUtility.Runtime.Renaming
{
    public static class BundleRenameUtility
    {
        public static bool RenameBundle(string srcFolder, HashSet<string> resourcePath,
            in Dictionary<string, HashSet<string>> bundleSceneMap, out SortedDictionary<string,string> resourceBundleMap)
        {
            return RenameGroupAssets(srcFolder, resourcePath, in bundleSceneMap, out resourceBundleMap);
        }
        
        public static bool RenameGroupResource(HashSet<string> resourcePaths,
            Dictionary<string,HashSet<string>> assetDependencyMap, out SortedDictionary<string, SortedDictionary<string,string>> resourceHashBundleMap)
        {
            resourceHashBundleMap = new SortedDictionary<string, SortedDictionary<string,string>>(StringComparer.Ordinal);
            foreach (var resourcePath in resourcePaths)
            {
                resourceHashBundleMap.Add(resourcePath, new SortedDictionary<string,string>());
            }
            
            foreach (var resourceBundleItem in assetDependencyMap)
            {
                foreach (var Dependency in resourceBundleItem.Value)
                {
                    if (!File.Exists(Dependency))
                    {
                        Debug.LogErrorFormat("Fail to rename file with hash, there is no file in path: {0}", Dependency);
                        return false;
                    }
                    if (!HashResourceBundle(Dependency,resourceBundleItem.Key, ref resourceHashBundleMap))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool RenameGroupAssets(string srcFolder,  HashSet<string> resourcePath,
            in Dictionary<string, HashSet<string>> bundleSceneMap, out SortedDictionary<string,string> resourceBundleMap)
        {
            resourceBundleMap = new SortedDictionary<string, string>();
            foreach (var scenePath in resourcePath)
            {
                resourceBundleMap.Add(scenePath,"");
            }

            foreach (var bundleSceneItem in bundleSceneMap)
            {
                string bundlePath = Path.Combine(srcFolder, bundleSceneItem.Key);
                if (!File.Exists(bundlePath))
                {
                    Debug.LogErrorFormat("Fail to rename file with hash, there is no file in path: {0}", bundlePath);
                    return false;
                }
                if (!HashBundle(bundlePath, bundleSceneItem.Value, ref resourceBundleMap))
                {
                    return false;
                }
            }
            return true;
        }
        
        private static bool HashResourceBundle(string assetPath, string resourcePath,
            ref SortedDictionary<string, SortedDictionary<string,string>> resourceHashBundleMap)
        {
            if (!GetHashID(assetPath, out string hashID))
            {
                return false;
            }
            
            if (!resourceHashBundleMap.TryGetValue(resourcePath, out SortedDictionary<string,string> bundleHashIDSingleSceneGroup))
            {
                Debug.LogError("Fail to rename file with hash, there is internal error during key-value inverse process");
                return false;
            }
            bundleHashIDSingleSceneGroup.Add(hashID,assetPath);
            
            return true;
        }

        private static bool HashBundle(string bundlePath, HashSet<string> realtiveScenes,
            ref SortedDictionary<string,string> resourceHashBundleMap)
        {
            if (!GetHashID(bundlePath, out string hashID))
            {
                return false;
            }
            foreach (var realtiveScene in realtiveScenes)
            {
                if (!resourceHashBundleMap.TryGetValue(realtiveScene, out string bundleHashIDSingleSceneGroup))
                {
                    Debug.LogError("Fail to rename file with hash, there is internal error during key-value inverse process");
                    return false;
                }
                resourceHashBundleMap[realtiveScene] = hashID;
            }

            try
            {
                File.Move(bundlePath, Path.Combine(Path.GetDirectoryName(bundlePath), hashID));
            }
            catch (IOException e)
            {
                Debug.LogErrorFormat("Fail to rename file: {0}, IOException: {1}", bundlePath, e.Message);
                return false;
            }
            return true;
        }

        private static bool GetHashID(string bundlePath, out string hashID)
        {
            hashID = string.Empty;
            try
            {
                using (FileStream fileStream = File.Open(bundlePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (SHA256 hasher = SHA256.Create())
                    {
                        fileStream.Position = 0;
                        byte[] hashBytes = hasher.ComputeHash(fileStream);
                        hashID = BitConverter.ToString(hashBytes).Replace("-", "");
                    }
                }
            }
            catch (IOException e)
            {
                Debug.LogErrorFormat("Fail to compute hash with file in path: {0},  IOException: {1}", bundlePath, e.Message);
                return false;
            }
            return true;
        }
    }
}
