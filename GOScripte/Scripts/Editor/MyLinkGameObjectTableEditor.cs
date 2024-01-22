using System.Collections;
using System.Collections.Generic;
using MyLink.GameObjectTable.Editor;
using MyLink.GameObjectTable.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyLink.GameObjectTable.Editor
{
    public class MyLinkGameObjectTableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Search All"))
            {
                MyLinkGameObjectTableExtension.InvokeAllGetMethod(SceneManager.GetActiveScene());
            }

            base.OnInspectorGUI();
        }
    }

}
