#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;

namespace UGC.PackagerUtility.Runtime.Bundle
{
    public static class BundleParameterUtility
    {
        public static BundleBuildParameters GetBundleBuildParameters(string outputFolder, bool isRebuild, BuildTarget buildTarget)
        {
            BuildTargetGroup buildGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            BundleBuildParameters parameters = new BundleBuildParameters(buildTarget, buildGroup, outputFolder);

            parameters.AppendHash = false;
            parameters.ContiguousBundles = true;
            parameters.DisableVisibleSubAssetRepresentations = true;

            parameters.UseCache = !isRebuild;
            parameters.WriteLinkXML = false;
            parameters.BundleCompression = UnityEngine.BuildCompression.LZ4;

            parameters.NonRecursiveDependencies = true;
            return parameters;
        }
    }
}
#endif