using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.Security.Cryptography;
using UGC.PackagerUtility.Runtime.Bundle;
using UGC.PackagerUtility.Runtime.Defines;
using UGC.PackagerUtility.Runtime.Renaming;
#endif

namespace UGC.PackagerUtility.Runtime.Manifest
{
    [Serializable]
    public class SceneManifest : ManifestHandlerBase
    {
        public List<SceneBlockData> SceneBlockMap;

        public override IEnumerator LoadBundles(string manifestPath, string bundleFolderPath,
            List<string> loadedPaths, Action<bool, List<string>, Dictionary<string, AssetBundle>> completeCallback)
        {
            List<string> tempErrorPath = new List<string>();
            Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
            foreach (var sceneBlockData in SceneBlockMap)
            {
                if (loadedPaths.Contains(sceneBlockData.ManifestHashID))
                {
                    continue;
                }
                
                string bundleManifestPath =
                    Path.Combine(Path.Combine(bundleFolderPath, sceneBlockData.ManifestHashID + ".json"));
                byte[] bytes;
                using (FileStream fs = File.Open(bundleManifestPath, FileMode.Open))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                    fs.Dispose();
                }

                string json = Encoding.UTF8.GetString(bytes);
                BundleManifest bundleManifest = JsonUtility.FromJson<BundleManifest>(json);

                string bundlePath = Path.Combine(bundleFolderPath, bundleManifest.HashID);
                if (!File.Exists(bundlePath))
                {
                    tempErrorPath.Add(sceneBlockData.ManifestHashID);
                    continue;
                }

                AssetBundleCreateRequest tempBundle = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return tempBundle;
                if (tempBundle.isDone)
                {
                    bundles.Add(sceneBlockData.ManifestHashID, tempBundle.assetBundle);
                }
                else
                {
                    tempErrorPath.Add(sceneBlockData.ManifestHashID);
                }

                yield return null;
            }

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
            foreach (var sceneBlockData in SceneBlockMap)
            {
                allHashID.Add(sceneBlockData.ManifestHashID);
            }

            return allHashID;
        }

        public override string FindHashIDToName(string name)
        {
            string hashID = null;
            foreach (var data in SceneBlockMap)
            {
                if (data.ScenePath == name)
                {
                    hashID = data.ManifestHashID;
                }
            }

            return hashID;
        }

        public string GetStartScene()
        {
            for (int i = 0; i < SceneBlockMap.Count; i++)
            {
                if (SceneBlockMap[i].IsStartScene)
                {
                    return SceneBlockMap[i].ScenePath;
                }
            }

            Debug.LogError("Not Start Scene!");
            return null;
        }

        public string GetScenePathByHash(string hashID)
        {
            foreach (var sceneBlockData in SceneBlockMap)
            {
                if (sceneBlockData.ManifestHashID == hashID)
                {
                    return sceneBlockData.ScenePath;
                }
            }

            Debug.LogError("Not the hashID scene:" + hashID);
            return null;
        }

        public string GetHashID(int index)
        {
            foreach (var sceneBlockData in SceneBlockMap)
            {
                if (sceneBlockData.Index == index)
                {
                    return sceneBlockData.ManifestHashID;
                }
            }

            Debug.LogError("Not find number " + index + " HashID!");
            return null;
        }

#if UNITY_EDITOR
        public override bool BuildBundles(HashSet<string> scenesPath, BuildTarget buildTarget, string outputFolder,
            bool isForceRebuild, bool isClearOutputFolder)
        {
            Dictionary<HashSet<string>, HashSet<string>> assetSceneMap = GetAssetGroup(scenesPath);
            if (!BundleBuildUtility.BuildAssetBundle(assetSceneMap, isForceRebuild, isClearOutputFolder, buildTarget,
                    outputFolder, out Dictionary<string, HashSet<string>> bundleSceneMap))
            {
                return false;
            }

            if (!BundleRenameUtility.RenameBundle(outputFolder, scenesPath, in bundleSceneMap,
                    out SortedDictionary<string, string> resourceBundleMap))
            {
                return false;
            }

            if (!PreparatoryManifestFile(resourceBundleMap, outputFolder))
            {
                return false;
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            return true;
        }

        public void DeleteTempFolder(string scenesPath)
        {
            string tempFolder = Path.GetDirectoryName(scenesPath);
            FileUtil.DeleteFileOrDirectory(Path.GetFullPath(tempFolder));
            FileUtil.DeleteFileOrDirectory(Path.GetFullPath(tempFolder + ".meta"));
        }

        private HashSet<string> GetAllResources()
        {
            HashSet<string> resourcePaths = new HashSet<string>();
            foreach (var sceneBlockData in SceneBlockMap)
            {
                resourcePaths.Add(sceneBlockData.ScenePath);
            }

            return resourcePaths;
        }

        private Dictionary<HashSet<string>, HashSet<string>> GetAssetGroup(HashSet<string> resourcePaths)
        {
            if (!IsScenePathsValid(resourcePaths, ".unity"))
            {
                return null;
            }

            Dictionary<HashSet<string>, HashSet<string>> sceneDependencies =
                new Dictionary<HashSet<string>, HashSet<string>>();
            foreach (var sceneBlockData in resourcePaths)
            {
                HashSet<string> hashSet = new HashSet<string>();
                hashSet.Add(sceneBlockData);
                sceneDependencies.Add(hashSet, hashSet);
            }

            //TODO:Get light map
            return sceneDependencies;
        }

        private bool IsScenePathsValid(HashSet<string> resourcePaths, string lastName)
        {
            if (resourcePaths == null || resourcePaths.Count == 0)
            {
                Debug.LogError("The scenes to build AssetBundle can not be null!");
                return false;
            }

            foreach (string scenePath in resourcePaths)
            {
                string fullPath = Path.GetFullPath(scenePath);
                if (!File.Exists(fullPath) || Path.GetExtension(fullPath) != lastName)
                {
                    Debug.LogErrorFormat("Fail to assign AssetBundle, contain invalid scene path: {0}", fullPath);
                    return false;
                }
            }

            return true;
        }

        private bool PreparatoryManifestFile(SortedDictionary<string, string> resourceBundleMap, string outputFolder)
        {
            if (!IsGroupingStrategyValid(resourceBundleMap))
            {
                return false;
            }

            List<BundleManifest> sceneBundleDependencyList = GetBundleManifest(resourceBundleMap);

            Dictionary<string, string> resourceHashIDMap =
                WriteResourceManifest(sceneBundleDependencyList, outputFolder);
            if (!WriteSceneManifest(resourceHashIDMap, outputFolder))
            {
                return false;
            }

            return true;
        }

        private bool IsGroupingStrategyValid(SortedDictionary<string, string> resourceBundleMap)
        {
            if (resourceBundleMap == null)
            {
                Debug.LogErrorFormat("Fail to generate packing manifest, parameter: {0} can not be null !",
                    nameof(resourceBundleMap));
                return false;
            }

            foreach (var sceneBundleItem in resourceBundleMap)
            {
                if (sceneBundleItem.Value == null)
                {
                    Debug.LogErrorFormat(
                        "Fail to generate packing manifest, grouping strategy for {0} can not be null !",
                        nameof(sceneBundleItem.Key));
                    return false;
                }
            }

            return true;
        }

        private List<BundleManifest> GetBundleManifest(SortedDictionary<string, string> resourceBundleMap)
        {
            List<BundleManifest> sceneBundleDependencyList = new List<BundleManifest>();
            foreach (var sceneBundleItem in resourceBundleMap)
            {
                sceneBundleDependencyList.Add(new BundleManifest(sceneBundleItem.Key, sceneBundleItem.Value));
            }

            return sceneBundleDependencyList;
        }

        private Dictionary<string, string> WriteResourceManifest(List<BundleManifest> sceneBundleDependencyList,
            string outputFolder)
        {
            Dictionary<string, string> resourceHashIDMap = new Dictionary<string, string>();
            foreach (var sceneBlockData in sceneBundleDependencyList)
            {
                string hashID = "";
                using (FileStream fileStream = File.Open(sceneBlockData.ResourcePath, FileMode.Open, FileAccess.Read,
                           FileShare.None))
                {
                    using (SHA256 hasher = SHA256.Create())
                    {
                        fileStream.Position = 0;
                        byte[] hashBytes = hasher.ComputeHash(fileStream);
                        hashID = BitConverter.ToString(hashBytes).Replace("-", "");
                        resourceHashIDMap.Add(sceneBlockData.ResourcePath, hashID);
                    }
                }

                string jsonString = JsonUtility.ToJson(sceneBlockData, true);
                string outputPath = Path.Combine(outputFolder, hashID + ".json");

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
                    return null;
                }
            }

            return resourceHashIDMap;
        }

        private bool WriteSceneManifest(Dictionary<string, string> resourceHashIDMap, string outPutFolder)
        {
            if (resourceHashIDMap == null)
            {
                return false;
            }

            if (SceneBlockMap == null)
            {
                SceneBlockMap = new List<SceneBlockData>();
                foreach (var keyValuePair in resourceHashIDMap)
                {
                    SceneBlockData sceneBlockData = new SceneBlockData();
                    sceneBlockData.ScenePath = keyValuePair.Key;
                    sceneBlockData.ManifestHashID = keyValuePair.Value;
                    SceneBlockMap.Add(sceneBlockData);
                }
            }
            else
            {
                foreach (var sceneBlockData in SceneBlockMap)
                {
                    foreach (var keyValuePair in resourceHashIDMap)
                    {
                        if (sceneBlockData.ScenePath == keyValuePair.Key)
                        {
                            sceneBlockData.ManifestHashID = keyValuePair.Value;
                        }
                    }
                }
            }

            string jsonString = JsonUtility.ToJson(this, true);
            string outPutPath = Path.Combine(outPutFolder, RuntimeDefines.SceneBlocksMapJsonName);
            using (FileStream fs = new FileStream(outPutPath, FileMode.Create))
            {
                byte[] bs = Encoding.UTF8.GetBytes(jsonString);
                fs.Write(bs, 0, bs.Length);
                fs.Flush();
                fs.Close();
                fs.Dispose();
                Debug.Log("create json form:" + RuntimeDefines.SceneBlocksMapJsonName);
            }

            return true;
        }
#endif
    }

    [Serializable]
    public class SceneBlockData
    {
        public string ScenePath;
        public string ManifestHashID;
        public int Index;
        public bool IsStartScene;
        public List<string> TexturePaths;
        public List<float> BlockPoints;
    }
}