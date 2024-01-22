#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UGC.PackagerUtility.Runtime.Bundle
{
    public static class BundleBuildUtility
    {
        private static readonly string bundleNamePrefix = "bundle_";

        public static bool BuildAssetBundle(Dictionary<HashSet<string>, HashSet<string>> assetSceneMap, bool isForceRebuild,
            bool isClearOutputFolder, BuildTarget buildTarget,string outputFolder, out Dictionary<string, HashSet<string>> bundleSceneMap)
        {
            if (assetSceneMap == null || assetSceneMap.Count == 0)
            {
                Debug.LogErrorFormat("Fail to build AssetBundle, {0} can not be null or empty !", nameof(assetSceneMap));
                bundleSceneMap = null;
                return false;
            }

            if (!BundleCollectUtility.GetBundleContent(assetSceneMap, bundleNamePrefix, 
                    out IBundleBuildContent bundleBuildContent, out bundleSceneMap))
            {
                Debug.LogError("Fail to get bundle contnet");
                return false;
            }

            string tempFolder = FileUtil.GetUniqueTempPathInProject();
            if (!BuildBundle(tempFolder,isForceRebuild,isClearOutputFolder,buildTarget,bundleBuildContent,outputFolder))
            {
                return false;
            }
            
            if (!MoveBundle(tempFolder, outputFolder, isClearOutputFolder))
            {
                Debug.LogError("Fail to move bundle to specific folder");
                return false;
            }
            Debug.LogFormat("Bundle build in {0}", outputFolder);

            FileUtil.DeleteFileOrDirectory(tempFolder);
            return true;
        }
        
        public static bool BuildAssetBundle(SortedDictionary<string,string> resourceHashBundleMap, bool isForceRebuild,
            bool isClearOutputFolder, BuildTarget buildTarget,string outputFolder)
        {
            if (resourceHashBundleMap == null || resourceHashBundleMap.Count == 0)
            {
                Debug.LogErrorFormat("Fail to build AssetBundle, {0} can not be null or empty !", nameof(resourceHashBundleMap));
                return false;
            }

            if (!BundleCollectUtility.GetBundleContent(resourceHashBundleMap, out IBundleBuildContent bundleBuildContent,out List<string> bundleNames))
            {
                Debug.LogError("Fail to get bundle contnet");
                return false;
            }

            string tempFolder = FileUtil.GetUniqueTempPathInProject();
            if (!BuildBundle(tempFolder,isForceRebuild,isClearOutputFolder,buildTarget,bundleBuildContent,outputFolder))
            {
                return false;
            }
            if (!MoveBundleByName(tempFolder, outputFolder, isClearOutputFolder,bundleNames))
            {
                Debug.LogError("Fail to move bundle to specific folder");
                return false;
            }
            Debug.LogFormat("Bundle build in {0}", outputFolder);

            FileUtil.DeleteFileOrDirectory(tempFolder);
            return true;
        }

        private static bool BuildBundle(string tempFolder,bool isForceRebuild,bool isClearOutputFolder, BuildTarget buildTarget,IBundleBuildContent bundleBuildContent,string outputFolder)
        {
            IBundleBuildParameters bundleBuildParameters = BundleParameterUtility.GetBundleBuildParameters(tempFolder,
                isForceRebuild, buildTarget);
            
            ReturnCode returnCode = ContentPipeline.BuildAssetBundles(bundleBuildParameters, bundleBuildContent,
                out IBundleBuildResults _);
            if (returnCode != ReturnCode.Success)
            {
                Debug.LogErrorFormat("Fail to build AssetBundle, return code: {0}", returnCode);
                return false;
            }
            return true;
        }
        
        private static bool MoveBundleByName(string srcFolder, string destFolder, bool isClearOutputFolder,List<string> bundleNames)
        {
            if (!ExistsPath(srcFolder,destFolder,isClearOutputFolder))
            {
                Debug.LogError("Path error!");
                return false;
            }

            for (int i = 0; i < bundleNames.Count; i++)
            {
                string srcPath = Path.Combine(srcFolder,bundleNames[i]);
                string destPath = Path.Combine(destFolder,bundleNames[i]);
              
                if (File.Exists(destPath))
                {
                    try
                    {
                        File.Delete(destPath);
                    }
                    catch (IOException e)
                    {
                        Debug.LogErrorFormat("Fail to delete existing bundle in path : {0},  IOException: {1}", destPath, e.Message);
                        return false;
                    }
                }

                try
                {
                    File.Move(srcPath, destPath);
                }
                catch (IOException e)
                {
                    Debug.LogErrorFormat("Fail to move bundle from {0} to {1}, IOException: {2}", srcPath, destPath, e.Message);
                    return false;
                }
            }
            return true;
        }

        private static bool MoveBundle(string srcFolder, string destFolder, bool isClearOutputFolder)
        {
            if (!ExistsPath(srcFolder,destFolder,isClearOutputFolder))
            {
                Debug.LogError("Path error!");
                return false;
            }

            string bundleNamePattern = string.Format("{0}*", bundleNamePrefix);
            string[] bundlePaths = Directory.GetFiles(srcFolder, bundleNamePattern, SearchOption.TopDirectoryOnly);
            for (int i = 0; i < bundlePaths.Length; ++i)
            {
                string name = Path.GetFileName(bundlePaths[i]);
                string destPath = Path.Combine(Path.GetFullPath(destFolder), name);
                if (File.Exists(destPath))
                {
                    try
                    {
                        File.Delete(destPath);
                    }
                    catch (IOException e)
                    {
                        Debug.LogErrorFormat("Fail to delete existing bundle in path : {0},  IOException: {1}", destPath, e.Message);
                        return false;
                    }
                }

                try
                {
                    File.Move(bundlePaths[i], destPath);
                }
                catch (IOException e)
                {
                    Debug.LogErrorFormat("Fail to move bundle from {0} to {1}, IOException: {2}", bundlePaths[i], destPath, e.Message);
                    return false;
                }
            }
            return true;
        }

        private static bool ExistsPath(string srcFolder, string destFolder, bool isClearOutputFolder)
        {
            if (isClearOutputFolder)
            {
                if (Directory.Exists(destFolder))
                {
                    FileUtil.DeleteFileOrDirectory(destFolder);
                }
            }

            if (!Directory.Exists(destFolder))
            {
                try
                {
                    Directory.CreateDirectory(destFolder);
                }
                catch (IOException e)
                {
                    Debug.LogErrorFormat("Fail to create directory, IOException: {0}", e.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
#endif
