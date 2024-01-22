using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UGC.PackagerUtility.Runtime.Bundle;
using UGC.PackagerUtility.Runtime.Defines;
using UGC.PackagerUtility.Runtime.Renaming;
#endif

namespace UGC.PackagerUtility.Runtime.Manifest
{
    [Serializable]
    public class ResourceManifest : ManifestHandlerBase
    {
        public List<ResourceData> IconData;

        public override IEnumerator LoadBundles(string manifestPath, string bundleFolderPath,
            List<string> loadedPaths, Action<bool, List<string>, Dictionary<string, AssetBundle>> completeCallback)
        {
            List<string> tempErrorPath = new List<string>();
            Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
            foreach (var resourceData in IconData)
            {
                foreach (var variable in resourceData.PrefabMap)
                {
                    if (!loadedPaths.Contains(variable.PrefabID))
                    {
                        string bundlePath = Path.Combine(bundleFolderPath, variable.PrefabID);
                        if (!File.Exists(bundlePath))
                        {
                            tempErrorPath.Add(variable.PrefabID);
                            continue;
                        }

                        AssetBundleCreateRequest tempBundle = AssetBundle.LoadFromFileAsync(bundlePath);
                        yield return tempBundle;
                        if (tempBundle.isDone)
                        {
                            bundles.Add(variable.PrefabID, tempBundle.assetBundle);
                        }
                        else
                        {
                            tempErrorPath.Add(variable.PrefabID);
                        }
                    }
                }

                foreach (var variable in resourceData.MatMap)
                {
                    if (!loadedPaths.Contains(variable.MatHashID))
                    {
                        string bundlePath = Path.Combine(bundleFolderPath, variable.MatHashID);
                        if (!File.Exists(bundlePath))
                        {
                            tempErrorPath.Add(variable.MatHashID);
                            continue;
                        }

                        AssetBundleCreateRequest tempBundle = AssetBundle.LoadFromFileAsync(bundlePath);
                        yield return tempBundle;
                        if (tempBundle.isDone)
                        {
                            bundles.Add(variable.MatHashID, tempBundle.assetBundle);
                        }
                        else
                        {
                            tempErrorPath.Add(variable.MatHashID);
                        }
                    }
                }

                foreach (var variable in resourceData.ShaderMap)
                {
                    if (!loadedPaths.Contains(variable.ShaderHashID))
                    {
                        string bundlePath = Path.Combine(bundleFolderPath, variable.ShaderHashID);
                        if (!File.Exists(bundlePath))
                        {
                            tempErrorPath.Add(variable.ShaderHashID);
                            continue;
                        }

                        AssetBundleCreateRequest tempBundle = AssetBundle.LoadFromFileAsync(bundlePath);
                        yield return tempBundle;
                        if (tempBundle.isDone)
                        {
                            bundles.Add(variable.ShaderHashID, tempBundle.assetBundle);
                        }
                        else
                        {
                            tempErrorPath.Add(variable.ShaderHashID);
                        }
                    }
                }

                foreach (var variable in resourceData.MeshMap)
                {
                    if (!loadedPaths.Contains(variable.MeshHashID))
                    {
                        string bundlePath = Path.Combine(bundleFolderPath, variable.MeshHashID);
                        if (!File.Exists(bundlePath))
                        {
                            tempErrorPath.Add(variable.MeshHashID);
                            continue;
                        }

                        AssetBundleCreateRequest tempBundle = AssetBundle.LoadFromFileAsync(bundlePath);
                        yield return tempBundle;
                        if (tempBundle.isDone)
                        {
                            bundles.Add(variable.MeshHashID, tempBundle.assetBundle);
                        }
                        else
                        {
                            tempErrorPath.Add(variable.MeshHashID);
                        }
                    }
                }

                foreach (var variable in resourceData.TexMap)
                {
                    if (!loadedPaths.Contains(variable.TexHashID))
                    {
                        string bundlePath = Path.Combine(bundleFolderPath, variable.TexHashID);
                        if (!File.Exists(bundlePath))
                        {
                            tempErrorPath.Add(variable.TexHashID);
                            continue;
                        }

                        AssetBundleCreateRequest tempBundle = AssetBundle.LoadFromFileAsync(bundlePath);
                        yield return tempBundle;
                        if (tempBundle.isDone)
                        {
                            bundles.Add(variable.TexHashID, tempBundle.assetBundle);
                        }
                        else
                        {
                            tempErrorPath.Add(variable.TexHashID);
                        }
                    }
                }

                yield return null;
                if (tempErrorPath.Count != 0)
                {
                    completeCallback?.Invoke(false, tempErrorPath, bundles);
                    yield break;
                }

                completeCallback?.Invoke(true, tempErrorPath, bundles);
            }
        }

        public override bool LoadAsync(AssetBundle bundle, string key, out Object asset)
        {
            asset = bundle.LoadAsset(key);
            return true;
        }

        public override List<string> GetAllHashID()
        {
            List<string> allHashID = new List<string>();
            foreach (var data in IconData)
            {
                foreach (var map in data.TexMap)
                {
                    allHashID.Add(map.TexHashID);
                }

                foreach (var map in data.ShaderMap)
                {
                    allHashID.Add(map.ShaderHashID);
                }

                foreach (var map in data.MatMap)
                {
                    allHashID.Add(map.MatHashID);
                }

                foreach (var map in data.MeshMap)
                {
                    allHashID.Add(map.MeshHashID);
                }

                foreach (var map in data.PrefabMap)
                {
                    allHashID.Add(map.PrefabID);
                }
            }

            return allHashID;
        }

        public override string FindHashIDToName(string name)
        {
            string hashID = null;
            foreach (var data in IconData)
            {
                foreach (var map in data.PrefabMap)
                {
                    if (map.PrefabName == name)
                    {
                        hashID = map.PrefabID;
                    }
                }

                foreach (var map in data.MeshMap)
                {
                    if (map.MeshName == name)
                    {
                        hashID = map.MeshHashID;
                    }
                }

                foreach (var map in data.MatMap)
                {
                    if (map.MatName == name)
                    {
                        hashID = map.MatHashID;
                    }
                }

                foreach (var map in data.TexMap)
                {
                    if (map.TexName == name)
                    {
                        hashID = map.TexHashID;
                    }
                }

                foreach (var map in data.ShaderMap)
                {
                    if (map.ShaderName == name)
                    {
                        hashID = map.ShaderHashID;
                    }
                }
            }

            return hashID;
        }

        public IEnumerator LoadAsyncAllToManifest(Dictionary<string, AssetBundle> bundles,
            Action<bool, string, GameObject> completeCallback)
        {
            foreach (var resourceData in IconData)
            {
                foreach (var variable in resourceData.TexMap)
                {
                    if (!bundles.TryGetValue(FindHashIDToName(variable.TexName), out AssetBundle bundle))
                    {
                        Debug.LogError($"lost bundle!:{variable.TexHashID}");
                        completeCallback?.Invoke(false, "Texture is Null", null);
                    }

                    AssetBundleRequest tempBundle = bundle.LoadAssetAsync(variable.TexName);
                    yield return tempBundle;
                    if (!tempBundle.isDone)
                    {
                        Debug.LogError($"load bundle undone !:{variable.TexHashID}");
                        completeCallback?.Invoke(false, $"load bundle undone", null);
                    }
                }

                foreach (var variable in resourceData.ShaderMap)
                {
                    if (!bundles.TryGetValue(FindHashIDToName(variable.ShaderName), out AssetBundle bundle))
                    {
                        Debug.LogError($"lost bundle!:{variable.ShaderHashID}");
                        completeCallback?.Invoke(false, "Shader is Null", null);
                    }

                    AssetBundleRequest tempBundle = bundle.LoadAssetAsync(variable.ShaderName);
                    yield return tempBundle;
                    if (!tempBundle.isDone)
                    {
                        Debug.LogError($"load bundle undone !:{variable.ShaderHashID}");
                        completeCallback?.Invoke(false, "load bundle undone", null);
                    }
                }

                foreach (var variable in resourceData.MatMap)
                {
                    if (!bundles.TryGetValue(FindHashIDToName(variable.MatName), out AssetBundle bundle))
                    {
                        Debug.LogError($"lost bundle!:{variable.MatHashID}");
                        completeCallback?.Invoke(false, "Material is Null", null);
                    }

                    AssetBundleRequest tempBundle = bundle.LoadAssetAsync(variable.MatName);
                    yield return tempBundle;
                    if (!tempBundle.isDone)
                    {
                        Debug.LogError($"load bundle undone !:{variable.MatHashID}");
                        completeCallback?.Invoke(false, "load bundle undone", null);
                    }
                }

                foreach (var variable in resourceData.MeshMap)
                {
                    if (!bundles.TryGetValue(FindHashIDToName(variable.MeshName), out AssetBundle bundle))
                    {
                        Debug.LogError($"lost bundle!:{variable.MeshHashID}");
                        completeCallback?.Invoke(false, "Mesh is Null", null);
                    }

                    AssetBundleRequest tempBundle = bundle.LoadAssetAsync(variable.MeshName);
                    yield return tempBundle;
                    if (!tempBundle.isDone)
                    {
                        Debug.LogError($"load bundle undone !:{variable.MeshHashID}");
                        completeCallback?.Invoke(false, "load bundle undone", null);
                    }
                }

                GameObject prefab = null;
                foreach (var variable in resourceData.PrefabMap)
                {
                    if (!bundles.TryGetValue(FindHashIDToName(variable.PrefabName), out AssetBundle bundle))
                    {
                        Debug.LogError($"lost bundle!:{variable.PrefabID}");
                        completeCallback?.Invoke(false, "Prefab is Null", null);
                    }

                    AssetBundleRequest tempBundle = bundle.LoadAssetAsync(variable.PrefabName);
                    yield return tempBundle;
                    if (!tempBundle.isDone)
                    {
                        Debug.LogError($"load bundle undone !:{variable.PrefabID}");
                        completeCallback?.Invoke(false, "load bundle undone", null);
                    }

                    if (prefab == null)
                    {
                        prefab = tempBundle.asset as GameObject;
                    }
                }

                completeCallback?.Invoke(true, "is done", prefab);
            }
        }

#if UNITY_EDITOR
        public override bool BuildBundles(HashSet<string> prefabsPath, BuildTarget buildTarget, string outputFolder,
            bool isForceRebuild,
            bool isClearOutputFolder)
        {
            Dictionary<string, HashSet<string>> assetDependencyMap = GetAssetGroup(prefabsPath);
            if (!BundleRenameUtility.RenameGroupResource(prefabsPath, assetDependencyMap,
                    out SortedDictionary<string, SortedDictionary<string, string>> resourceHashBundleMap))
            {
                return false;
            }

            SortedDictionary<string, string> hashDependencyMap = GetHashDependencyMap(resourceHashBundleMap);

            if (!BundleBuildUtility.BuildAssetBundle(hashDependencyMap, isForceRebuild, isClearOutputFolder,
                    buildTarget, outputFolder))
            {
                return false;
            }

            if (!PreparatoryManifestFile(resourceHashBundleMap, outputFolder))
            {
                return false;
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            return true;
        }

        private SortedDictionary<string, string> GetHashDependencyMap(
            SortedDictionary<string, SortedDictionary<string, string>> resourceHashBundleMap)
        {
            SortedDictionary<string, string> hashDependencyMap = new SortedDictionary<string, string>();
            foreach (var assetHashBundleItem in resourceHashBundleMap)
            {
                foreach (var hashBundleItem in assetHashBundleItem.Value)
                {
                    if (!hashDependencyMap.ContainsKey(hashBundleItem.Key))
                    {
                        hashDependencyMap.Add(hashBundleItem.Key, hashBundleItem.Value);
                    }
                }
            }

            return hashDependencyMap;
        }

        private Dictionary<string, HashSet<string>> GetAssetGroup(HashSet<string> resourcesPath)
        {
            DependencyUtility dependencyUtility = new DependencyUtility();
            Dictionary<string, HashSet<string>> tempPaths = dependencyUtility.GetPrefabDependencies(resourcesPath);

            return tempPaths;
        }

        private bool PreparatoryManifestFile(
            SortedDictionary<string, SortedDictionary<string, string>> resourceHashBundleMap, string outputFolder)
        {
            if (!IsGroupingStrategyValid(resourceHashBundleMap))
            {
                return false;
            }

            List<BundleManifest> bundleDependencyList = GetBundleManifest(resourceHashBundleMap);

            WriteBundleManifest(bundleDependencyList, outputFolder);
            if (!WriteResourceManifest(resourceHashBundleMap, outputFolder))
            {
                return false;
            }

            return true;
        }

        private bool IsGroupingStrategyValid(
            SortedDictionary<string, SortedDictionary<string, string>> resourceHashBundleMap)
        {
            if (resourceHashBundleMap == null)
            {
                Debug.LogErrorFormat("Fail to generate packing manifest, parameter: {0} can not be null !",
                    nameof(resourceHashBundleMap));
                return false;
            }

            foreach (var sceneBundleItem in resourceHashBundleMap)
            {
                if (sceneBundleItem.Value == null || sceneBundleItem.Value.Count == 0)
                {
                    Debug.LogErrorFormat(
                        "Fail to generate packing manifest, grouping strategy for {0} can not be null !",
                        nameof(sceneBundleItem.Key));
                    return false;
                }
            }

            return true;
        }

        private List<BundleManifest> GetBundleManifest(
            SortedDictionary<string, SortedDictionary<string, string>> resourceHashBundleMap)
        {
            List<BundleManifest> sceneBundleDependencyList = new List<BundleManifest>();
            foreach (var sceneBundleItem in resourceHashBundleMap)
            {
                foreach (var VARIABLE in sceneBundleItem.Value)
                {
                    sceneBundleDependencyList.Add(new BundleManifest(VARIABLE.Value, VARIABLE.Key));
                }
            }

            return sceneBundleDependencyList;
        }

        private void WriteBundleManifest(List<BundleManifest> BundleDependencyList, string outputFolder)
        {
            foreach (var VARIABLE in BundleDependencyList)
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

        private bool WriteResourceManifest(
            SortedDictionary<string, SortedDictionary<string, string>> resourceHashBundleMap, string outPutFolder)
        {
            if (resourceHashBundleMap == null)
            {
                return false;
            }

            ResourceManifest resourceManifest = new ResourceManifest();
            resourceManifest.IconData = new List<ResourceData>();


            foreach (var resourceHashBundleItem in resourceHashBundleMap)
            {
                ResourceData resourceData = new ResourceData();
                resourceData.PrefabName = resourceHashBundleItem.Key;

                foreach (var hashBundleItem in resourceHashBundleItem.Value)
                {
                    switch (Path.GetExtension(hashBundleItem.Value))
                    {
                        case ".prefab":
                            PrefabData prefabData = new PrefabData();
                            prefabData.PrefabName = hashBundleItem.Value;
                            prefabData.PrefabID = hashBundleItem.Key;
                            if (resourceData.PrefabMap == null)
                            {
                                resourceData.PrefabMap = new List<PrefabData>();
                            }

                            resourceData.PrefabMap.Add(prefabData);
                            break;

                        case ".mat":
                            MatData matData = new MatData();
                            matData.MatName = hashBundleItem.Value;
                            matData.MatHashID = hashBundleItem.Key;
                            if (resourceData.MatMap == null)
                            {
                                resourceData.MatMap = new List<MatData>();
                            }

                            resourceData.MatMap.Add(matData);
                            break;

                        case ".fbx":
                            MeshData meshData = new MeshData();
                            meshData.MeshName = hashBundleItem.Value;
                            meshData.MeshHashID = hashBundleItem.Key;
                            if (resourceData.MeshMap == null)
                            {
                                resourceData.MeshMap = new List<MeshData>();
                            }

                            resourceData.MeshMap.Add(meshData);
                            break;

                        case ".TGA":
                            TexData texData = new TexData();
                            texData.TexName = hashBundleItem.Value;
                            texData.TexHashID = hashBundleItem.Key;
                            if (resourceData.TexMap == null)
                            {
                                resourceData.TexMap = new List<TexData>();
                            }

                            resourceData.TexMap.Add(texData);
                            break;

                        case ".png":
                            TexData pngData = new TexData();
                            pngData.TexName = hashBundleItem.Value;
                            pngData.TexHashID = hashBundleItem.Key;
                            if (resourceData.TexMap == null)
                            {
                                resourceData.TexMap = new List<TexData>();
                            }

                            resourceData.TexMap.Add(pngData);
                            break;

                        case ".shader":
                            ShaderData shaderData = new ShaderData();
                            shaderData.ShaderName = hashBundleItem.Value;
                            shaderData.ShaderHashID = hashBundleItem.Key;
                            if (resourceData.ShaderMap == null)
                            {
                                resourceData.ShaderMap = new List<ShaderData>();
                            }

                            resourceData.ShaderMap.Add(shaderData);
                            break;

                        case ".shadergraph":
                            ShaderData shadergraphData = new ShaderData();
                            shadergraphData.ShaderName = hashBundleItem.Value;
                            shadergraphData.ShaderHashID = hashBundleItem.Key;
                            if (resourceData.ShaderMap == null)
                            {
                                resourceData.ShaderMap = new List<ShaderData>();
                            }

                            resourceData.ShaderMap.Add(shadergraphData);
                            break;
                    }
                }

                resourceManifest.IconData.Add(resourceData);
            }

            string jsonString = JsonUtility.ToJson(resourceManifest, true);
            string outPutPath = Path.Combine(outPutFolder, RuntimeDefines.ResourceMapJsonName);
            using (FileStream fs = new FileStream(outPutPath, FileMode.Create))
            {
                byte[] bs = Encoding.UTF8.GetBytes(jsonString);
                fs.Write(bs, 0, bs.Length);
                fs.Flush();
                fs.Close();
                fs.Dispose();
                Debug.Log("create json form:" + RuntimeDefines.ResourceMapJsonName);
            }

            return true;
        }
#endif
    }

    [Serializable]
    public class ResourceData
    {
        public string PrefabName;

        public List<PrefabData> PrefabMap;

        public List<MatData> MatMap;

        public List<ShaderData> ShaderMap;

        public List<MeshData> MeshMap;

        public List<TexData> TexMap;
    }


    [Serializable]
    public class PrefabData
    {
        public string PrefabName;
        public string PrefabID;
    }

    [Serializable]
    public class MatData
    {
        public string MatName;
        public string MatHashID;
    }

    [Serializable]
    public class ShaderData
    {
        public string ShaderName;
        public string ShaderHashID;
    }

    [Serializable]
    public class MeshData
    {
        public string MeshName;
        public string MeshHashID;
    }

    [Serializable]
    public class TexData
    {
        public string TexName;
        public string TexHashID;
    }
}