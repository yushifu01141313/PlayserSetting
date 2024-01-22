#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UGC.PackagerUtility.Runtime.Defines;

namespace UGC.PackagerUtility.Runtime.Manifest
{
    public class DependencyUtility
    {
        public Dictionary<string, HashSet<string>> GetPrefabDependencies(HashSet<string> resourcesPath)
        {
            Dictionary<string, HashSet<string>> prefabsDependencies = new Dictionary<string, HashSet<string>>();

            foreach (var resourcePath in resourcesPath)
            {
                HashSet<string> prefabDependencies = new HashSet<string>();
                GetPrefabDependencyByPath(resourcePath, prefabDependencies);
                prefabsDependencies.Add(resourcePath, prefabDependencies);
            }

            if (prefabsDependencies.Count < 1)
            {
                Debug.LogError("Scenes Path error! Or the scenes inexistence!");
                return null;
            }

            return prefabsDependencies;
        }

        private void GetPrefabDependencyByPath(string resourcePath, HashSet<string> prefabsDependencies)
        {
            string[] dependencies = AssetDatabase.GetDependencies(resourcePath, true);
            for (int i = 0; i < dependencies.Length; ++i)
            {
                Debug.LogError(dependencies[i]);
                if (!DependencySuffixNames.SuffixNames.Contains(Path.GetExtension(dependencies[i])))
                {
                    continue;
                }

                if (!prefabsDependencies.Contains(dependencies[i]))
                {
                    prefabsDependencies.Add(dependencies[i]);
                }
            }
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
    }
}
#endif