using System.IO;
using System.Text;
using UnityEngine;

namespace UGC.PackagerUtility.Runtime.Manifest
{
    public static class ManifestDeserializeUtility
    {
        public static bool GetBundleManifest(string manifestPath, out string manifest)
        {
            if(!IsPathValid(manifestPath))
            {
                manifest = null;
                return false;
            }

            if (!DeserializeBundleManifest(manifestPath, out manifest))
            {
                return false;
            }

            return true;
        }

        public static bool GetBundleManifest(byte[] buffer, out string manifestJson)
        {
            if (!IsBytesValid(buffer))
            {
                manifestJson = null;
                return false;
            }
            manifestJson = Encoding.UTF8.GetString(buffer);
            return true;
        }

        private static bool IsPathValid(string manifestPath)
        {
            if (string.IsNullOrEmpty(manifestPath))
            {
                Debug.LogErrorFormat("Fail to deserializa bundle manifest file, {0} can't be null or empty !", nameof(manifestPath));
                return false;
            }
            if (!File.Exists(manifestPath))
            {
                Debug.LogErrorFormat("Fail to deserializa bundle manifest file, there is no file in path: {0}", manifestPath);
                return false;
            }

            return true;
        }

        private static bool IsBytesValid(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                Debug.LogErrorFormat("Fail to deserializa bundle manifest bytes, parameter: {0} can't be null or empty!", nameof(buffer));
                return false;
            }

            return true;
        }

        private static bool DeserializeBundleManifest(string manifestPath, out string manifestJson)
        {
            try
            {
                string manifestFullPath = Path.GetFullPath(manifestPath);
                using (FileStream fileStream = File.Open(manifestFullPath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    byte[] buffer = new byte[fileStream.Length];
                    int cnt = fileStream.Read(buffer, 0, buffer.Length);
                    manifestJson = Encoding.UTF8.GetString(buffer);
                 
                }
            }
            catch (IOException e)
            {
                Debug.LogErrorFormat("Fail to deserialize bundle manifest file in path: {0}, IOException: {1}", manifestPath, e.Message);
                manifestJson = null;
                return false;
            }

            return true;
        }
    }
}
