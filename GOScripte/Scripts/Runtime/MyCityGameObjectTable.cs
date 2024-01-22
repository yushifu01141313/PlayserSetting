using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyLink.GameObjectTable.Runtime
{
    public class MyCityGameObjectTable : MyLinkGameObjectTableSingleton<MyCityGameObjectTable>, IMyLinkGameObjectTable
    {

        [SerializeField]
        private GameObject volumeDay;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyCItyGlobalVolume()
        {
#if UNITY_EDITOR
            volumeDay = GameObject.Find("volume_Day");
            return volumeDay;
#else
            return volumeDay;
#endif
        }

        [SerializeField]
        private GameObject directionalLight;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyCItyDirectionalLight()
        {
#if UNITY_EDITOR
            directionalLight = GameObject.Find("Directional Light");

            return directionalLight;
#else
            return directionalLight;
#endif
        }

        [SerializeField]
        private GameObject wave;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyCityWave()
        {
#if UNITY_EDITOR
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject rootObject in rootObjects)
            {
                if (rootObject.name == "wave")
                {
                    wave = rootObject;
                    break;
                }
            }

            return wave;
#else
            return wave;
#endif
        }
        
        [SerializeField]
        private GameObject streetUIName;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyCItyStreetUIName()
        {
#if UNITY_EDITOR
            if (!streetUIName)
            {
                streetUIName = GameObject.Find("Street_UI_Name");
            }

            return streetUIName;
#else
            
            return streetUIName;
#endif
        }
        [SerializeField]
        private GameObject dayFX;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyCItyDayFX()
        {
#if UNITY_EDITOR
            GameObject FX = GameObject.Find("FX");
            dayFX = FX.transform.Find("FX_Day").gameObject;

            return dayFX;
#else
            return dayFX;
#endif
        }
        [SerializeField]
        private GameObject nightFX;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyCItyNightFX()
        {
#if UNITY_EDITOR
            GameObject FX = GameObject.Find("FX");
            nightFX = FX.transform.Find("FX_Night").gameObject;

            return nightFX;
#else
            return nightFX;
#endif
        }
        [SerializeField]
        private GameObject centerPoint;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyCItyCenterPoint()
        {
#if UNITY_EDITOR
            centerPoint = GameObject.Find("CenterPoint");

            return centerPoint;
#else
            return centerPoint;
#endif
        }
        [SerializeField]
        private GameObject flagPathPointParent;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyCItyNpcFlagPathPointParent()
        {
#if UNITY_EDITOR
            flagPathPointParent = GameObject.Find("FlagPathPointParent");

            return flagPathPointParent;
#else
            return flagPathPointParent;
#endif
        }

        [SerializeField]
        private List<Texture2D> Night_Lightmap;

        [GameObjectTableEditorInvoke]
        public List<Texture2D> GetMyCityNightLightmap()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return Night_Lightmap;
            }
            Night_Lightmap = new();
            string[] hdrPaths =
            {
                "MyCity_LM0_final.hdr",
                "MyCity_LMA1_final.hdr",
                "MyCity_LMA2_final.hdr",
                "MyCity_LMA3_final.hdr",
                "MyCity_LMA4_final.hdr"
            };

            foreach (string path in hdrPaths)
            {
                Texture2D asset = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/BakeryLightmaps/Night_Lightmap", path), typeof(Texture2D));
                Night_Lightmap.Add(asset);
            }

            return Night_Lightmap;
#else
            return Night_Lightmap;
#endif
        }

        [SerializeField]
        private List<Texture2D> Day_Lightmap;

        [GameObjectTableEditorInvoke]
        public List<Texture2D> GetMyCityDayLightmap()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return Day_Lightmap;
            }
            Day_Lightmap = new();
            string[] hdrPaths =
            {
                "MyCity_LM0_final.hdr",
                "MyCity_LMA1_final.hdr",
                "MyCity_LMA2_final.hdr",
                "MyCity_LMA3_final.hdr",
                "MyCity_LMA4_final.hdr"
            };

            foreach (string path in hdrPaths)
            {
                Texture2D asset = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/BakeryLightmaps/Day_Lightmap", path), typeof(Texture2D));
                Day_Lightmap.Add(asset);
            }

            return Day_Lightmap;
#else
            return Day_Lightmap;
#endif
        }

        [SerializeField] private GameObject buildingItemParent;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyCItyBuildingItemParent()
        {
#if UNITY_EDITOR
            if (!buildingItemParent)
            {
                buildingItemParent = GameObject.Find("architecture");
            }

            return buildingItemParent;
#else
            return buildingItemParent;
#endif
        }
    }
}
