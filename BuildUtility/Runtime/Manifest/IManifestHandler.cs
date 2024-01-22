using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UGC.PackagerUtility.Runtime.Manifest
{
    public interface IManifestHandler
    {
#if UNITY_EDITOR
        public bool BuildBundles(HashSet<string> scenesPath, BuildTarget buildTarget, string outputFolder,
            bool isForceRebuild, bool isClearOutputFolder);
#endif
        public abstract IEnumerator LoadBundles(string manifestPath, string bundleFolderPath,
            List<string> loadedPaths, Action<bool, List<string>, Dictionary<string, AssetBundle>> completeCallback);

        public IEnumerator LoadBundle(string bundleFolderPath,
            Action<bool, string[]> completeCallback, params string[] bundleNames);

        public abstract bool LoadAsync(AssetBundle bundle, string key, out Object asset);
    }
}