using System.IO;
using System.Reflection;

namespace MyLink.GameObjectTable.Runtime
{
    public class DataPath
    {
#if UNITY_EDITOR
        public static readonly string PackagePath = UnityEditor.PackageManager.PackageInfo.FindForAssembly(Assembly.GetAssembly(typeof(DataPath))).assetPath;
        public static readonly string PrefabsPath = Path.Combine(PackagePath, "Prefabs");
        public static readonly string AvatarSpawnerPath = Path.Combine(PrefabsPath, "AvatarSpawner.prefab");
#endif
    }
}
