// -----------------------------------------------------------------------------
//
// Use this sample example C# file to develop samples to guide usage of APIs
// in your package.
//
// -----------------------------------------------------------------------------

using System;
using System.Collections;
using CommonFramework.Runtime;
using UGC.ResourceFramework.Runtime.Events;
using UGC.ResourceFramework.Runtime.Services;
using UnityEngine;

namespace UGC.ResourceFramework.Samples.BuildBundleSample.Runtime
{
    public class LoadBundleExample : MonoSingleton<LoadBundleExample>
    {
        private Guid lastDownloadTime;

        //TODO:Change to the bundle path you want to load.
        private readonly string sceneManifestPath =
            "Assets/Samples/ResourceFramework/0.0.2/BuildBundleSample/SampleObjects/SceneBlocksMap.json";

        private readonly string resourceManifestPath =
            "Assets/Samples/ResourceFramework/0.0.2/BuildBundleSample/SampleResourceObjects/ResourceMap.json";

        private readonly string sceneFolderPath =
            "Assets/Samples/ResourceFramework/0.0.2/BuildBundleSample/SampleObjects";

        private readonly string resourceFolderPath =
            "Assets/Samples/ResourceFramework/0.0.2/BuildBundleSample/SampleResourceObjects";


        private GameObject prefab;
        private GameObject go;

        public void Awake()
        {
            //TODO:Register an Load Bundle Event.
            ResourceService.CreateInstance();

            EventCenter.StartListenToEvent<LoadBundlesAndAllAssetCompleteEvent>(LoadAsyncCompleteCallback);
            EventCenter.StartListenToEvent<UnLoadBundlesForManifestCompleteEvent>(UnLoadBundleCompleteCallback);

            //StartCoroutine(StartSceneSample());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("S tartResourceSample");
                StartResourceSample();
            }

            if (Input.GetKeyDown(KeyCode.U))
            { 
                Debug.Log("U nloadBundleByManifest");
                UnloadBundleByManifest();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("I nstantiatePrefab");
                InstantiatePrefab();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("D estroyPrefab");
                DestroyPrefab();
            }
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("U nloadUnusedAssets");
                Resources.UnloadUnusedAssets();
            }
        }

        protected override void OnDestroyInstance()
        {
            EventCenter.StopListenToEvent<LoadBundlesAndAllAssetCompleteEvent>(LoadAsyncCompleteCallback);
            EventCenter.StopListenToEvent<UnLoadBundlesForManifestCompleteEvent>(UnLoadBundleCompleteCallback);
        }

        private void UnLoadBundleCompleteCallback(UnLoadBundlesForManifestCompleteEvent loadAsyncAllCompleteEvent)
        {
            if (!loadAsyncAllCompleteEvent.Result)
            {
                Debug.LogError("Load Async error!");
                return;
            }

            Debug.Log($"----------- UnLoad Bundle Success ------------");
        }

        private void LoadAsyncCompleteCallback(LoadBundlesAndAllAssetCompleteEvent loadAsyncAllCompleteEvent)
        {
            if (!loadAsyncAllCompleteEvent.IsDone)
            {
                Debug.LogError("Load Async error!");
                return;
            }

            Debug.Log($"----------- Load Async Success ------------");
            Debug.Log($"loading {loadAsyncAllCompleteEvent.Prefab.name}");

            prefab = loadAsyncAllCompleteEvent.Prefab;
            go = Instantiate(prefab);
            go.transform.position = new Vector3(0, 0, 0);
        }

        private void DestroyPrefab()
        {
            Debug.Log("DestroyPrefab");
            Destroy(go);
        }

        private void InstantiatePrefab()
        {
            Debug.Log("InstantiatePrefab");
            go = Instantiate(prefab);
            go.transform.position = new Vector3(0, 0, 0);
        }

        private void UnloadBundleByManifest()
        {
            Debug.Log("UnloadBundleByManifest");
            lastDownloadTime = Guid.NewGuid();
            UnLoadBundlesForManifestRequestEvent loadBundlesRequestEvent =
                new UnLoadBundlesForManifestRequestEvent(lastDownloadTime, resourceManifestPath);
            EventCenter.TriggerEvent(loadBundlesRequestEvent);
        }

        private void StartResourceSample()
        {
            Debug.Log("StartResourceSample");
            lastDownloadTime = Guid.NewGuid();
            LoadBundlesAndAllAssetRequestEvent loadBundlesRequestEvent =
                new LoadBundlesAndAllAssetRequestEvent(lastDownloadTime, resourceManifestPath, resourceFolderPath);
            EventCenter.TriggerEvent(loadBundlesRequestEvent);
        }

        private IEnumerator StartSceneSample()
        {
            lastDownloadTime = Guid.NewGuid();

            LoadBundlesRequestEvent loadBundlesRequestEvent =
                new LoadBundlesRequestEvent(lastDownloadTime, sceneManifestPath, ManifestTypes.Scene, sceneFolderPath);
            EventCenter.TriggerEvent(loadBundlesRequestEvent);
            yield return new WaitForSeconds(1);

            LoadAsyncRequestEvent loadAsyncRequestEvent =
                new LoadAsyncRequestEvent(lastDownloadTime, ManifestTypes.Scene,
                    "Assets/Blocks/block13.unity");
            EventCenter.TriggerEvent(loadAsyncRequestEvent);
        }
    }
}