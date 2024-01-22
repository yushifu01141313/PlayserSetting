using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommonFramework.Runtime;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace MyLink.GameObjectTable.Runtime
{
    public class MyHomeGameObjectTable : MyLinkGameObjectTableSingleton<MyHomeGameObjectTable>, IMyLinkGameObjectTable
    {
        [SerializeField] private GameObject myHomeWeatherWindow;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyHomeWeatherWindow()
        {
#if UNITY_EDITOR
            myHomeWeatherWindow = GameObject.Find("M_MyHome_Curtain_03");

            return myHomeWeatherWindow;
#else
            return myHomeWeatherWindow;
#endif
        }

        [SerializeField] private GameObject myHomeBedroomWindow;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyHomeBedroomWindow()
        {
#if UNITY_EDITOR
            myHomeBedroomWindow = GameObject.Find("M_MyHome_Curtain_03 (1)");

            return myHomeBedroomWindow;
#else
            return myHomeBedroomWindow;
#endif
        }

        [SerializeField] private GameObject windowLight;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyHomeWindowLight()
        {
#if UNITY_EDITOR
            windowLight = GameObject.Find("Tyndall");

            return windowLight;
#else
            return windowLight;
#endif
        }


        [SerializeField] private GameObject avatarSpawner;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyHomeAvatarSpawner()
        {
#if UNITY_EDITOR
            avatarSpawner = AssetDatabase.LoadAssetAtPath<GameObject>(DataPath.AvatarSpawnerPath);

            return avatarSpawner;
#else
            return avatarSpawner;
#endif
        }

        [SerializeField] private GameObject livingGround;

        [GameObjectTableEditorInvoke]
        public GameObject GetLivingGround()
        {
#if UNITY_EDITOR
            livingGround = GameObject.Find("Pb_MyHome_LivingRoom_Floor");
            return livingGround;
#else
            return livingGround;
#endif
        }

        [SerializeField] private GameObject bedRoomGround;

        [GameObjectTableEditorInvoke]
        public GameObject GetBedRoomGround()
        {
#if UNITY_EDITOR
            bedRoomGround = GameObject.Find("Pb_MyHome_BedRoom_Floor");
            return bedRoomGround;
#else
            return bedRoomGround;
#endif
        }

        [SerializeField] private List<GameObject> baseRoomWalls;

        [GameObjectTableEditorInvoke]
        public List<GameObject> GetBaseRoomWalls()
        {
#if UNITY_EDITOR
            string[] newBaseRoomWalls =
            {
                "Pb_MyHome_LivingRoom_Wall01",
                "Pb_MyHome_LivingRoom_Wall02",
                "Pb_MyHome_LivingRoom_Wall03",
                "Pb_MyHome_LivingRoom_Wall04",
                "Pb_MyHome_BedRoom_Wall01",
                "Pb_MyHome_BedRoom_Wall02",
                "Pb_MyHome_BedRoom_Wall03",
                "Pb_MyHome_BedRoom_Wall04",
            };
            baseRoomWalls = new List<GameObject>();
            baseRoomWalls.Clear();
            foreach (var baseRoomWall in newBaseRoomWalls)
            {
                baseRoomWalls.Add(GameObject.Find(baseRoomWall));
            }
            return baseRoomWalls;
#else
            return baseRoomWalls;
#endif
        }

        [SerializeField] private List<GameObject> baseGroundFurnitures;

        [GameObjectTableEditorInvoke]
        public List<GameObject> GetBaseGroundFurnitures()
        {
#if UNITY_EDITOR
            string[] newBaseGroundFurnitures =
            {
                "Pb_MyHome_Carpet_01",
                "Pb_MyHome_Wall_Cabinet_030",
                "Pb_MyHome_Wall_Cabinet_029",
                "Pb_MyHome_Entertainment_01",
                "Pb_MyHome_Entertainment_01 (1)"
            };
            baseGroundFurnitures = new List<GameObject>();
            baseGroundFurnitures.Clear();
            foreach (var baseGroundFurniture in newBaseGroundFurnitures)
            {
                baseGroundFurnitures.Add(GameObject.Find(baseGroundFurniture));
            }

            return baseGroundFurnitures;
#else
            return baseGroundFurnitures;
#endif
        }

        [SerializeField] private List<GameObject> baseWallFurnitures;

        [GameObjectTableEditorInvoke]
        public List<GameObject> GetBaseWallFurnitures()
        {
#if UNITY_EDITOR
            string[] newBaseWallFurnitures =
            {
                "BedRoomWindows",
                "LivingRoom_Windows",
                "Pb_MyHome_Picture_04",
                "M_MyHome_Walllamp_01"
            };
            baseWallFurnitures = new List<GameObject>();
            baseWallFurnitures.Clear();
            foreach (var baseWallFurniture in newBaseWallFurnitures)
            {
                baseWallFurnitures.Add(GameObject.Find(baseWallFurniture));
            }

            return baseWallFurnitures;
#else
            return baseWallFurnitures;
#endif
        }

        [SerializeField] private List<GameObject> doorWalls;

        [GameObjectTableEditorInvoke]
        public List<GameObject> GetDoorWalls()
        {
#if UNITY_EDITOR
            string[] newDoorWalls =
            {
                "Pb_MyHome_LivingRoom_Wall02",
                "Pb_MyHome_BedRoom_Wall01"
            };
            doorWalls = new List<GameObject>();
            doorWalls.Clear();
            foreach (var doorWall in newDoorWalls)
            {
                doorWalls.Add(GameObject.Find(doorWall));
            }

            return doorWalls;
#else
            return doorWalls;
#endif
        }

        [SerializeField] private GameObject doorTwo;

        [GameObjectTableEditorInvoke]
        public GameObject GetDoorTwo()
        {
#if UNITY_EDITOR
            doorTwo = GameObject.Find("M_MyHome_Door_Frame02 (1)");
            return doorTwo;
#else
            return doorTwo;
#endif
        }

        [SerializeField] private GameObject doorOneWall;

        [GameObjectTableEditorInvoke]
        public GameObject GetDoorOneWall()
        {
#if UNITY_EDITOR
            doorOneWall = GameObject.Find("Pb_MyHome_LivingRoom_Wall03");
            return doorOneWall;
#else
            return doorOneWall;
#endif
        }

        [SerializeField] private GameObject livingWallOne;

        [GameObjectTableEditorInvoke]
        public GameObject GetLivingWallOne()
        {
#if UNITY_EDITOR
            livingWallOne = GameObject.Find("Pb_MyHome_LivingRoom_Wall01");
            return livingWallOne;
#else
            return livingWallOne;
#endif
        }


        [SerializeField] private GameObject doorFrame;

        [GameObjectTableEditorInvoke]
        public GameObject GetDoorFrame()
        {
#if UNITY_EDITOR
            doorFrame = GameObject.Find("M_MyHome_Door_Frame02");
            return doorFrame;
#else
            return doorFrame;
#endif
        }

        [SerializeField] private GameObject doorOne;

        [GameObjectTableEditorInvoke]
        public GameObject GetDoorOne()
        {
#if UNITY_EDITOR
            doorOne = GameObject.Find("M_MyHome_Door_Frame02 (2)");
            return doorOne;
#else
            return doorOne;
#endif
        }

        [SerializeField] private GameObject enterNFTTrigger;

        [GameObjectTableEditorInvoke]
        public GameObject GetEnterNFTTrigger()
        {
#if UNITY_EDITOR
            enterNFTTrigger = GameObject.Find("EnterNFTTrigger");

            return enterNFTTrigger;
#else
            return enterNFTTrigger;
#endif
        }

        [SerializeField] private GameObject exitNFTTrigger;

        [GameObjectTableEditorInvoke]
        public GameObject GetExitNFTTrigger()
        {
#if UNITY_EDITOR
            exitNFTTrigger = GameObject.Find("ExitNFTTrigger");

            return exitNFTTrigger;
#else
            return exitNFTTrigger;
#endif
        }

        [SerializeField] private GameObject nftCorridorTrigger;

        [GameObjectTableEditorInvoke]
        public GameObject GetNftCorridorTrigger()
        {
#if UNITY_EDITOR
            nftCorridorTrigger = GameObject.Find("NFTCorridorTrigger");

            return nftCorridorTrigger;
#else
            return nftCorridorTrigger;
#endif
        }
        
        [SerializeField] private GameObject shoeCabinet;

        [GameObjectTableEditorInvoke]
        public GameObject GetShoeCabinet()
        {
#if UNITY_EDITOR
            shoeCabinet = GameObject.Find("Pb_MyHome_Xiegui");

            return shoeCabinet;
#else
            return shoeCabinet;
#endif
        }
        
        [SerializeField] private GameObject cactus;

        [GameObjectTableEditorInvoke]
        public GameObject GetCactus()
        {
#if UNITY_EDITOR
            cactus = GameObject.Find("M_MyHome_Cactus_01");

            return cactus;
#else
            return cactus;
#endif
        }
        
        [SerializeField] private GameObject picture;

        [GameObjectTableEditorInvoke]
        public GameObject GetPicture()
        {
#if UNITY_EDITOR
            picture = GameObject.Find("Pb_MyHome_Picture_04");

            return picture;
#else
            return picture;
#endif
        }
        
        [SerializeField] private GameObject raccoon;

        [GameObjectTableEditorInvoke]
        public GameObject GetRaccoon()
        {
#if UNITY_EDITOR
            raccoon = GameObject.Find("Raccoon");

            return raccoon;
#else
            return raccoon;
#endif
        }
        
        [SerializeField] private GameObject tv;

        [GameObjectTableEditorInvoke]
        public GameObject GetTV()
        {
#if UNITY_EDITOR
            tv = GameObject.Find("Pb_MyHome_TV_01");

            return tv;
#else
            return tv;
#endif
        }

        [SerializeField] private GameObject gameController;

        [GameObjectTableEditorInvoke]
        public GameObject GetGameController()
        {
#if UNITY_EDITOR
            gameController = GameObject.Find("Pb_MyHome_GameController_01");
            if (gameController == null)
            {
                GameObject placeProp = GameObject.Find("Place_Prop");
                if (placeProp != null)
                {
                    for (int i = 0; i < placeProp.transform.childCount; i++)
                    {
                        var childTransform = placeProp.transform.GetChild(i);
                        if (childTransform.name.Equals("Pb_MyHome_GameController_01"))
                        {
                            gameController = childTransform.gameObject;
                            break;
                        }
                    }
                }
            }

            return gameController;
#else
            return gameController;
#endif
        }

        [SerializeField] private GameObject enterCityTrigger;

        [GameObjectTableEditorInvoke]
        public GameObject GetEnterCityTrigger()
        {
#if UNITY_EDITOR
            enterCityTrigger = GameObject.Find("EnterCityTrigger");

            return enterCityTrigger;
#else
            return enterCityTrigger;
#endif
        }
        
        [SerializeField] private GameObject myHomeGlobalVolume;

        [GameObjectTableEditorInvoke]
        public GameObject GetMyHomeGlobalVolume()
        {
#if UNITY_EDITOR
            myHomeGlobalVolume = GameObject.Find("Global Volume");

            return myHomeGlobalVolume;
#else
            return myHomeGlobalVolume;
#endif
        }
        
        [SerializeField] private GameObject boxGameObject;

        [GameObjectTableEditorInvoke]
        public GameObject GetBoxGameObject()
        {
#if UNITY_EDITOR
            boxGameObject = GameObject.Find("Pb_MyHome_Baoxiang");

            return boxGameObject;
#else
                    return boxGameObject;
#endif
        }
        [SerializeField]
        private List<Cubemap> windowCubmap;

        [GameObjectTableEditorInvoke]
        public List<Cubemap> GetWindowCubeMap()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return windowCubmap;
            }
            windowCubmap = new();
            string[] cubemapPaths =
            {
                "T_Home_CloudyDCubemap.exr",
                "T_Home_CloudyNCubemap.exr",
                "T_Home_DayCubemap.exr",
                "T_Home_NightCubemap.exr",
            };

            foreach (string path in cubemapPaths)
            {
                Cubemap asset = (Cubemap)AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/WeatherSysteam/CubeMaps", path), typeof(Cubemap));
                windowCubmap.Add(asset);
            }

            return windowCubmap;
#else
            return windowCubmap;
#endif
        }
        
        [SerializeField] private GameObject enterHomeTrigger;

        [GameObjectTableEditorInvoke]
        public GameObject GetEnterHomeTrigger()
        {
#if UNITY_EDITOR
            enterHomeTrigger = GameObject.Find("EnterHomeTrigger");

            return enterHomeTrigger;
#else
            return enterHomeTrigger;
#endif
        }
        
        [SerializeField] private GameObject NTFSensorTrigger;

        [GameObjectTableEditorInvoke]
        public GameObject GetNTFSensorTrigger()
        {
#if UNITY_EDITOR
            NTFSensorTrigger = GameObject.Find("NFTSensorTrigger");

            return NTFSensorTrigger;
#else
            return NTFSensorTrigger;
#endif
        }

        [SerializeField] private GameObject floorQuad;

        [GameObjectTableEditorInvoke]
        public GameObject GetFloorQuadLight()
        {
#if UNITY_EDITOR
            floorQuad = GameObject.Find("Quad");

            return floorQuad;
#else
            return floorQuad;
#endif
        }
        
        [SerializeField] private GameObject nftRoomDoor;

        [GameObjectTableEditorInvoke]
        public GameObject GetNftRoomDoor()
        {
#if UNITY_EDITOR
            nftRoomDoor = GameObject.Find("M_MyHome_Door_Frame02 (2)");

            return nftRoomDoor;
#else
            return nftRoomDoor;
#endif
        }
        
        [SerializeField] private GameObject bedRoomDoor;

        [GameObjectTableEditorInvoke]
        public GameObject GetBedRoomDoor()
        {
#if UNITY_EDITOR
            bedRoomDoor = GameObject.Find("M_MyHome_Door_Frame02 (1)");

            return bedRoomDoor;
#else
            return bedRoomDoor;
#endif
        }
    }
}