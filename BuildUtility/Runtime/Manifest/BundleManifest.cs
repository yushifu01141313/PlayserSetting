using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.Security.Cryptography;
using UGC.PackagerUtility.Runtime.Bundle;
#endif

namespace UGC.PackagerUtility.Runtime.Manifest
{
    [Serializable]
    public class BundleManifest : ManifestHandlerBase
    {
        public string ResourcePath;

        public string HashID;

        public BundleManifest()
        {
            ResourcePath = string.Empty;
            HashID = string.Empty;
        }

        public BundleManifest(string resourcePath, string hashID)
        {
            ResourcePath = resourcePath;
            HashID = hashID;
        }

        public override IEnumerator LoadBundles(string manifestPath, string bundleFolderPath,
            List<string> loadedPaths, Action<bool, List<string>, Dictionary<string, AssetBundle>> completeCallback)
        {
            List<string> tempErrorPath = new List<string>();
            Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
            string bundlePath = Path.Combine(Path.GetDirectoryName(bundleFolderPath), HashID);
            if (!File.Exists(bundlePath))
            {
                tempErrorPath.Add(HashID);
                completeCallback?.Invoke(false, tempErrorPath, null);
                yield break;
            }

            AssetBundleCreateRequest tempBundle = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return tempBundle;
            bundles.Add(HashID, tempBundle.assetBundle);
            yield return null;
            if (tempErrorPath.Count != 0)
            {
                completeCallback?.Invoke(false, tempErrorPath, bundles);
                yield break;
            }

            completeCallback?.Invoke(true, tempErrorPath, bundles);
        }

        public override bool LoadAsync(AssetBundle bundle, string key, out Object asset)
        {
            asset = bundle.LoadAsset(key);
            return true;
        }

        public override List<string> GetAllHashID()
        {
            List<string> allHashID = new List<string>();
            allHashID.Add(HashID);
            return allHashID;
        }

        public override string FindHashIDToName(string name)
        {
            string hashID = null;
            if (ResourcePath == name)
            {
                hashID = HashID;
            }

            return hashID;
        }

#if UNITY_EDITOR
        public override bool BuildBundles(HashSet<string> prefabsPath, BuildTarget buildTarget, string outputFolder,
            bool isForceRebuild, bool isClearOutputFolder)
        {
            SortedDictionary<string, string> hashDependencyMap = GetHashDependencyMap(prefabsPath);

            if (!BundleBuildUtility.BuildAssetBundle(hashDependencyMap, isForceRebuild, isClearOutputFolder,
                    buildTarget, outputFolder))
            {
                return false;
            }

            if (!PreparatoryManifestFile(hashDependencyMap, outputFolder))
            {
                return false;
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            return true;
        }

        private bool PreparatoryManifestFile(SortedDictionary<string, string> hashDependencyMap, string outputFolder)
        {
            List<BundleManifest> bundleDependencyList = GetBundleManifest(hashDependencyMap);

            WriteBundleManifest(bundleDependencyList, outputFolder);

            return true;
        }

        private List<BundleManifest> GetBundleManifest(SortedDictionary<string, string> hashDependencyMap)
        {
            List<BundleManifest> bundleDependencyList = new List<BundleManifest>();
            foreach (var sceneBundleItem in hashDependencyMap)
            {
                bundleDependencyList.Add(new BundleManifest(sceneBundleItem.Value, sceneBundleItem.Key));
            }

            return bundleDependencyList;
        }

        private void WriteBundleManifest(List<BundleManifest> bundleDependencyList, string outputFolder)
        {
            foreach (var VARIABLE in bundleDependencyList)
            {
                string jsonString = JsonUtility.ToJson(VARIABLE, true);
                string outputPath = Path.Combine(outputFolder, VARIABLE.HashID + ".json");

                try
                {
                    using (FileStream fileStream =
                           File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(jsonString);
                        fileStream.Write(buffer, 0, buffer.Length);
                    }
                }
                catch (IOException e)
                {
                    Debug.LogErrorFormat("Fail to write packing manifest, IOException: {0}", e.Message);
                }
            }
        }

        private SortedDictionary<string, string> GetHashDependencyMap(HashSet<string> prefabsPath)
        {
            SortedDictionary<string, string> hashDependencyMap = new SortedDictionary<string, string>();
            foreach (var path in prefabsPath)
            {
                string hashID = string.Empty;
                using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (SHA256 hasher = SHA256.Create())
                    {
                        fileStream.Position = 0;
                        byte[] hashBytes = hasher.ComputeHash(fileStream);
                        hashID = BitConverter.ToString(hashBytes).Replace("-", "");
                        hashDependencyMap.Add(hashID, path);
                    }
                }
            }

            return hashDependencyMap;
        }

#endif
    }
}