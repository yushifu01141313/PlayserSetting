using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyLink.GameObjectTable.Runtime
{
    public class MyLinkGameObjectTableSingleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        
        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    instance = GetGameObjectTableFormCurrentScene();
                    if (instance == null)
                    {
                        Debug.LogError($"Failed to find any gameobject table in Current Scene {SceneManager.GetActiveScene().name}");
                        return null;
                    }
                }

                return instance;
            }
        }

        private static T GetGameObjectTableFormCurrentScene()
        {
            var currentScene = SceneManager.GetActiveScene();
            
            return GetGameObjectTableFromScene(currentScene);
        }

        /// <summary>
        /// Get the first gameObject table object from root gameobject of current scene
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static T GetGameObjectTableFromScene(Scene scene)
        {
            var rootGos = scene.GetRootGameObjects();   
            foreach (var rootGo in rootGos)
            {
                if (rootGo.TryGetComponent(out T table))
                {
                    return table;
                }
            }
            return null;
        }

        private void OnDestroy()
        {
            OnDestroyPreprocess();
            instance = null;
        }

        protected virtual void OnDestroyPreprocess()
        {
            
        }
    }
}
