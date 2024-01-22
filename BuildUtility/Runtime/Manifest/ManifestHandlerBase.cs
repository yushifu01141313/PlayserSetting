using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UGC.PackagerUtility.Runtime.Manifest
{
    public abstract class ManifestHandlerBase : IManifestHandler
    {
        /*protected static Dictionary<string, AssetBundle> allBundles = new Dictionary<string, AssetBundle>();
        protected static Dictionary<string, int> usedBundles = new Dictionary<string, int>();
        protected static HashSet<string> loadingKey = new HashSet<string>();*/

        //protected Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

        public abstract IEnumerator LoadBundles(string manifestPath, string bundleFolderPath,
            List<string> loadedPaths, Action<bool, List<string>, Dictionary<string, AssetBundle>> completeCallback);

        public IEnumerator LoadBundle(string bundleFolderPath, Action<bool, string[]> completeCallback,
            params string[] bundleNames)
        {
            List<string> errorPath = new List<string>();
            Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
            foreach (var loadName in bundleNames)
            {
                string tempName = Path.Combine(bundleFolderPath, loadName);
                if (!File.Exists(tempName))
                {
                    errorPath.Add(tempName);
                    continue;
                }

                AssetBundleCreateRequest tempBundle = AssetBundle.LoadFromFileAsync(tempName);
                yield return tempBundle;
                bundles.Add(loadName, tempBundle.assetBundle);
                yield return new WaitForEndOfFrame();
            }

            if (errorPath.Count > 0)
            {
                completeCallback?.Invoke(false, errorPath.ToArray());
                yield break;
            }

            completeCallback?.Invoke(true, new string[] { });
        }

        public abstract bool LoadAsync(AssetBundle bundle, string key, out Object asset);

        public abstract List<string> GetAllHashID();
        public abstract string FindHashIDToName(string name);

        public IEnumerator UnLoadBundle(AssetBundle bundle, string key, Action<bool> completeCallback)
        {
            AsyncOperation bundleOperation = bundle.UnloadAsync(true);
            yield return bundleOperation;
            if (bundleOperation.isDone)
            {
                completeCallback?.Invoke(true);
                yield break;
            }

            completeCallback?.Invoke(false);
        }

#if UNITY_EDITOR
        public abstract bool BuildBundles(HashSet<string> scenesPath, BuildTarget buildTarget, string outputFolder,
            bool isForceRebuild, bool isClearOutputFolder);

        protected Dictionary<HashSet<string>, HashSet<string>> CreateDependenciesMap(
            Dictionary<string, HashSet<string>> sceneDependencies)
        {
            if (sceneDependencies.Count < 1)
            {
                Debug.LogError("Scenes Path error! Or the scenes inexistence!");
                return null;
            }

            Dictionary<HashSet<string>, HashSet<string>> prefabsAndScenesDependencies =
                new Dictionary<HashSet<string>, HashSet<string>>();
            MergeHashset(sceneDependencies, prefabsAndScenesDependencies);

            prefabsAndScenesDependencies = RotateData(prefabsAndScenesDependencies);

            return prefabsAndScenesDependencies;
        }

        private void MergeHashset(Dictionary<string, HashSet<string>> sceneDependencies,
            Dictionary<HashSet<string>, HashSet<string>> outDependencies)
        {
            foreach (var VARIABLE in sceneDependencies)
            {
                if (outDependencies.ContainsKey(VARIABLE.Value))
                {
                    outDependencies[VARIABLE.Value].Add(VARIABLE.Key);
                }
                else
                {
                    HashSet<string> temp = new HashSet<string>();
                    temp.Add(VARIABLE.Key);
                    outDependencies.Add(VARIABLE.Value, temp);
                }
            }
        }

        private Dictionary<HashSet<string>, HashSet<string>> RotateData(
            Dictionary<HashSet<string>, HashSet<string>> tempPrefabsAndScenesDependencies)
        {
            Dictionary<HashSet<string>, HashSet<string>> prefabsAndScenesDependencies =
                new Dictionary<HashSet<string>, HashSet<string>>();
            foreach (var VARIABLE in tempPrefabsAndScenesDependencies)
            {
                prefabsAndScenesDependencies.Add(VARIABLE.Value, VARIABLE.Key);
            }

            return prefabsAndScenesDependencies;
        }

        protected bool OutputFolderPreprocess(string outputFolder)
        {
            if (string.IsNullOrEmpty(outputFolder))
            {
                return false;
            }

            if (Directory.Exists(outputFolder))
            {
                FileUtil.DeleteFileOrDirectory(outputFolder);
            }

            try
            {
                Directory.CreateDirectory(outputFolder);
            }
            catch (IOException e)
            {
                Debug.LogErrorFormat("Fail to create derectory, IOException: {0}", e.Message);
                return false;
            }

            return true;
        }
#endif
    }
}