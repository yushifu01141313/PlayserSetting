using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyLink.GameObjectTable.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyLink.GameObjectTable.Editor
{
    public class MyLinkGameObjectTableManualWindow : EditorWindow
    {

        private static GUIContent windowTitle = new GUIContent("GameObject Table");

        private IMyLinkGameObjectTable target;
        private List<Type> subclassList;

        [MenuItem("MyLink Framework/Developer/GameObject Table Manual")]
        private static void CreateManualWindow()
        {
            var window = EditorWindow.CreateWindow<MyLinkGameObjectTableManualWindow>();
            window.titleContent = windowTitle;
            window.Show();
        }

        private void OnEnable()
        {
            List<GameObject> rootGos = new();
            SceneManager.GetActiveScene().GetRootGameObjects(rootGos);
            var mylinkGOTables = rootGos.FindAll(a =>
                a.TryGetComponent(out IMyLinkGameObjectTable table));

            if (mylinkGOTables.Count == 1)
            {
                target = mylinkGOTables.First().GetComponent<IMyLinkGameObjectTable>();
            }

            if (target == null)
            {
                subclassList = GetSubClass(typeof(IMyLinkGameObjectTable));
            }
        }
        
        /// <summary>
        /// Get all subclass be derived from specify interface in assembly
        /// </summary>
        /// <param name="parentType">type</param>
        /// <returns>all sub-class</returns>
        public static List<Type> GetSubClass(Type parentType)
        {
            var subTypeList = new List<Type>();
            var assembly = parentType.Assembly;
            var assemblyAllTypes = assembly.GetTypes();
            foreach (var itemType in assemblyAllTypes)
            {
                if (itemType.GetInterface(parentType.Name) != null)
                {
                    subTypeList.Add(itemType);
                }
            }
            return subTypeList;
        }


        private void OnGUI()
        {
            if (target == null)
            {
                DrawGenerateOneTable();
            }
            else
            {
                DrawSearchAll();
            }
        }

        private void DrawGenerateOneTable()
        {
            GUILayout.Label("Select Table To Create");
            
            foreach (var subClass in subclassList)
            {
                if (GUILayout.Button(subClass.Name))
                {
                    var go = new GameObject(subClass.Name);

                    target = go.AddComponent(subClass) as IMyLinkGameObjectTable;

                    if (target == null)
                    {
                        continue;
                    }
                    
                    EditorGUIUtility.PingObject(go);
                    EditorUtility.SetDirty(go);
                    MyLinkGameObjectTableExtension.InvokeAllGetMethod(SceneManager.GetActiveScene());
                }     
            }
        }

        private void DrawSearchAll()
        {
            GUILayout.Label("Manually Search");
            if (GUILayout.Button("Search All"))
            {
                MyLinkGameObjectTableExtension.InvokeAllGetMethod(SceneManager.GetActiveScene());
            }
        }
    }
}
