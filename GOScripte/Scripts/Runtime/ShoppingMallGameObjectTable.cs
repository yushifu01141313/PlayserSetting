#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace MyLink.GameObjectTable.Runtime
{
    public class ShoppingMallGameObjectTable : MyLinkGameObjectTableSingleton<ShoppingMallGameObjectTable>,
        IMyLinkGameObjectTable
    {
        [SerializeField] private GameObject avatarSpawner;

        [GameObjectTableEditorInvoke]
        public GameObject GetAvatarSpawner()
        {
#if UNITY_EDITOR
            avatarSpawner = AssetDatabase.LoadAssetAtPath<GameObject>(DataPath.AvatarSpawnerPath);

            return avatarSpawner;
#else
            return avatarSpawner;
#endif
        }
        
        [SerializeField] private GameObject shoppingMallGlobalVolume;

        [GameObjectTableEditorInvoke]
        public GameObject GetShoppingMallGlobalVolume()
        {
#if UNITY_EDITOR
            shoppingMallGlobalVolume = GameObject.Find("Global Volume");

            return shoppingMallGlobalVolume;
#else
            return shoppingMallGlobalVolume;
#endif
        }
    }
}