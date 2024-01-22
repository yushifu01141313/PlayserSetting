using System;
using System.Collections.Generic;
using System.Linq;
using MyLink.GameObjectTable.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyLink.GameObjectTable.Editor
{
    public class MyLinkGameObjectTableExtension
    {
             
#if UNITY_EDITOR
        /// <summary>
        /// design for artist tool
        /// This function can automatically load this objects needed by the program.
        /// The find logic in specific by developer
        /// </summary>
        public static bool InvokeAllGetMethod(Scene scene)
        {
            List<GameObject> rootGos = new();
            scene.GetRootGameObjects(rootGos);
            var mylinkGOTables = rootGos.FindAll((a) =>
            {
                return a.TryGetComponent(out IMyLinkGameObjectTable table);
            });
            // var mylinkGOTables = FindObjectsOfType<MyLinkGameObjectTable<T>>();

            if (mylinkGOTables.Count == 0)
            {
                Debug.LogError("Failed to find any MyLinkGameObjectTable Object in scene.");
                return false;
            }
            
            if (mylinkGOTables.Count > 1)
            {
                Debug.LogError("Multiple Objects of MyLinkGameObjectTable were found in scene. Only one object is supported");
                return false;
            }

            var mylinkGoTable = mylinkGOTables[0].GetComponent<IMyLinkGameObjectTable>();
            var methodInfos = mylinkGoTable.GetType().GetMethods();

            bool hasError = false;

            var methods = methodInfos.Where((a) =>
            {
                return a.CustomAttributes.Any(attr =>
                    attr.AttributeType == typeof(GameObjectTableEditorInvokeAttribute)
                );
            });

            foreach (var method in methods)
            {
                try
                {
                    var getResult = method.Invoke(mylinkGoTable, new object[]{});

                    if (getResult == null)
                    {
                        Debug.LogError($"Failed to get specify GameObject. Check method: {method.Name}");
                        hasError = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to execute search method. Detail : {e}");
                    hasError = true;
                }
                
            }

            return !hasError;
        }
#endif
    }
}
