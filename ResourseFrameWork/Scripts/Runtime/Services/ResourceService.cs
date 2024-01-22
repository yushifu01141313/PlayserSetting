using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommonFramework.Runtime;
using UGC.PackagerUtility.Runtime.Manifest;
using UGC.ResourceFramework.Runtime.Events;
using UnityEngine;
using Object = UnityEngine.Object;
using UGC.ResourceFramework.Runtime.Defines;

namespace UGC.ResourceFramework.Runtime.Services
{
    public class ResourceService : MonoSingleton<ResourceService>
    {
        //key:manifest local path,value:manifest
        private Dictionary<string, ManifestHandlerBase> manifestMap = new Dictionary<string, ManifestHandlerBase>();

        //key:hashID,value:Bundle
        private Dictionary<string, AssetBundle> allBundles = new Dictionary<string, AssetBundle>();

        //key:hashID,value:number of use
        private Dictionary<string, int> usedBundles = new Dictionary<string, int>();

        //key:hashID
        private HashSet<string> loadingKey = new HashSet<string>();

        //Delete the bundle if it has not been referenced for a while
        private Queue<string> bundleUnloadQueue = new Queue<string>();

        private bool checkQueueSwitch;
        private string endMark = RuntimeDefines.EndMark;
        private string manifestMark = RuntimeDefines.ManifestMark;

        private void Awake()
        {
            EventCenter.StartListenToEvent<LoadBundlesAndAllAssetRequestEvent>(LoadBundlesAndAllAssetRequestHandle);
            EventCenter.StartListenToEvent<UnLoadBundlesForManifestRequestEvent>(UnLoadBundleForManifestHandle);
            EventCenter.StartListenToEvent<LoadBundlesRequestEvent>(LoadManifestHandle);
            EventCenter.StartListenToEvent<LoadAsyncRequestEvent>(LoadAsyncHandle);
            EventCenter.StartListenToEvent<WarmUpForSceneRequestEvent>(WarmUpHandle);
            EventCenter.StartListenToEvent<GetAllManifestPathRequestEvent>(GetAllManifestHandle);
            EventCenter.StartListenToEvent<UnLoadBundleRequestEvent>(UnLoadBundleHandle);
            EventCenter.StartListenToEvent<LoadBundleRequestEvent>(LoadBundleHandle);
            EventCenter.StartListenToEvent<MultipleLoadAsyncRequestEvent>(LoadAsyncByList);
            EventCenter.StartListenToEvent<LoadAsyncAllRequestEvent>(LoadAsyncAllRequestHandle);
            EventCenter.StartListenToEvent<UnLoadAllBundlesRequestEvent>(UnloadAllBundleAndList);

            checkQueueSwitch = true;
            StartCoroutine(CheckBundleUnloadQueue());
        }

        protected override void OnDestroyInstance()
        {
            EventCenter.StopListenToEvent<LoadBundlesAndAllAssetRequestEvent>(LoadBundlesAndAllAssetRequestHandle);
            EventCenter.StopListenToEvent<UnLoadBundlesForManifestRequestEvent>(UnLoadBundleForManifestHandle);
            EventCenter.StopListenToEvent<LoadBundlesRequestEvent>(LoadManifestHandle);
            EventCenter.StopListenToEvent<LoadAsyncRequestEvent>(LoadAsyncHandle);
            EventCenter.StopListenToEvent<WarmUpForSceneRequestEvent>(WarmUpHandle);
            EventCenter.StopListenToEvent<GetAllManifestPathRequestEvent>(GetAllManifestHandle);
            EventCenter.StopListenToEvent<UnLoadBundleRequestEvent>(UnLoadBundleHandle);
            EventCenter.StopListenToEvent<LoadBundleRequestEvent>(LoadBundleHandle);
            EventCenter.StopListenToEvent<MultipleLoadAsyncRequestEvent>(LoadAsyncByList);
            EventCenter.StopListenToEvent<LoadAsyncAllRequestEvent>(LoadAsyncAllRequestHandle);
            EventCenter.StopListenToEvent<UnLoadAllBundlesRequestEvent>(UnloadAllBundleAndList);

            checkQueueSwitch = false;
            StopCoroutine(CheckBundleUnloadQueue());
        }

        private void UnloadAllBundleAndList(UnLoadAllBundlesRequestEvent requestEvent)
        {
            manifestMap.Clear();
            allBundles.Clear();
            usedBundles.Clear();
            bundleUnloadQueue.Clear();
            loadingKey.Clear();
            AssetBundle.UnloadAllAssetBundles(requestEvent.UnloadAllObjects);
        }

        private void LoadBundlesAndAllAssetRequestHandle(LoadBundlesAndAllAssetRequestEvent requestEvent)
        {
            ManifestHandlerBase iManifest = FindManifestToPath(requestEvent.ManifestPath);

            LoadBundlesRequestEvent bundlesRequestEventEvent = new LoadBundlesRequestEvent(requestEvent.RequestID,
                requestEvent.ManifestPath, ManifestTypes.Resource, requestEvent.BundleFolderPath);

            LoadBundleByManifest<ResourceManifest>(iManifest, requestEvent.ManifestPath,
                requestEvent.BundleFolderPath, bundlesRequestEventEvent, LoadBundlesResourceCompleteCallback);
        }

        private void LoadBundleByManifest<T>(ManifestHandlerBase iManifest, string manifestPath,
            string bundleFolderPath, LoadBundlesRequestEvent bundlesRequestEventEvent,
            Action<Guid, bool, IReadOnlyList<string>, string, ManifestHandlerBase, Dictionary<string, AssetBundle>>
                callbacks)
            where T : ManifestHandlerBase
        {
            T resourceManifest = (T)iManifest;
            if (iManifest == null)
            {
                resourceManifest = (T)GetManifest<T>(iManifest, bundlesRequestEventEvent);
                StartCoroutine(resourceManifest.LoadBundles(manifestPath, bundleFolderPath,
                    FindLoadedID<T>(resourceManifest),
                    (condition, errorPaths, loadedBundles) =>
                    {
                        callbacks?.Invoke(bundlesRequestEventEvent.RequestID, condition, errorPaths,
                            manifestPath, resourceManifest, loadedBundles);
                    }));
                return;
            }

            callbacks?.Invoke(bundlesRequestEventEvent.RequestID, true, new List<string>(), manifestPath,
                resourceManifest, new Dictionary<string, AssetBundle>());
        }

        private void LoadBundlesResourceCompleteCallback(Guid requestID, bool result, IReadOnlyList<string> errorPaths,
            string manifestPath,
            ManifestHandlerBase manifestInstance, Dictionary<string, AssetBundle> loadedBundles)
        {
            SetLoadedBundles(loadedBundles, errorPaths);
            ResourceManifest resourceManifest = manifestInstance as ResourceManifest;
            List<string> names = resourceManifest.GetAllHashID();
            WaitForLoadingBundles(names,
                () => ContinueLoadBundle(names, requestID, manifestInstance as ResourceManifest, true));
        }

        private void SetLoadedBundles(Dictionary<string, AssetBundle> loadedBundles, IReadOnlyList<string> errorPaths)
        {
            foreach (var bundleData in loadedBundles)
            {
                allBundles.Add(bundleData.Key, bundleData.Value);
                loadingKey.Remove(bundleData.Key);
            }

            foreach (var errorPath in errorPaths)
            {
                loadingKey.Remove(errorPath);
            }
        }

        private void ContinueLoadBundle(List<string> names, Guid requestID, ResourceManifest iManifest, bool autoFlow)
        {
            Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

            foreach (var hashID in names)
            {
                AssetBundle bundle = GetBundle(hashID);
                if (bundle != null)
                {
                    bundles.Add(hashID, bundle);
                }
            }

            if (bundles.Count <= 0)
            {
                LoadAsyncAllCompleteEvent loadAsyncAllCompleteEvent =
                    new LoadAsyncAllCompleteEvent(requestID, false, null);
                EventCenter.TriggerEvent(loadAsyncAllCompleteEvent);
            }


            if (autoFlow)
            {
                StartCoroutine((iManifest as ResourceManifest)?.LoadAsyncAllToManifest(bundles,
                    (isDone, message, prefab) =>
                        LoadedBundlesAndLoadAsyncAllRequestCallback(isDone, message, prefab, requestID)));
            }
            else
            {
                StartCoroutine((iManifest as ResourceManifest)?.LoadAsyncAllToManifest(bundles,
                    (isDone, message, prefab) =>
                        LoadAsyncAllCompleteCallback(isDone, message, prefab, requestID)));
            }

            RecordUseBundles(bundles);
        }

        private void RecordUseBundles(Dictionary<string, AssetBundle> bundles)
        {
            foreach (var bundleData in bundles)
            {
                if (usedBundles.ContainsKey(bundleData.Key))
                {
                    int number = usedBundles[bundleData.Key];
                    usedBundles[bundleData.Key] = ++number;
                    continue;
                }

                usedBundles.Add(bundleData.Key, 1);
            }
        }

        private void UnLoadBundleForManifestHandle(UnLoadBundlesForManifestRequestEvent requestEvent)
        {
            string unLoadManifestPath = requestEvent.UnLoadManifestPath;
            ManifestHandlerBase iManifest = FindManifestToPath(unLoadManifestPath);
            if (iManifest == null)
            {
                Debug.LogError($"Not find manifest:{unLoadManifestPath}!");
                return;
            }

            UnloadUseBundleByManifestHandle(iManifest, requestEvent);
        }

        private void UnloadUseBundleByManifestHandle(ManifestHandlerBase iManifest,
            UnLoadBundlesForManifestRequestEvent requestEvent)
        {
            List<string> allHashID = new List<string>();
            if (iManifest is ResourceManifest)
            {
                allHashID = (iManifest as ResourceManifest).GetAllHashID();
            }

            if (iManifest is SceneManifest)
            {
                allHashID = (iManifest as SceneManifest).GetAllHashID();
            }

            if (iManifest is BundleManifest)
            {
                allHashID = (iManifest as BundleManifest).GetAllHashID();
            }

            List<string> hashIDs = new List<string>();
            foreach (var hashID in allHashID)
            {
                if (!CheckBundleNumber(hashID))
                {
                    continue;
                }

                hashIDs.Add(hashID);
            }

            UnloadBundleForQueue(hashIDs, requestEvent.UnLoadManifestPath);
            UnLoadBundlesForManifestCompleteEvent unLoadBundleCompleteEvent =
                new UnLoadBundlesForManifestCompleteEvent(requestEvent.RequestID, true);
            EventCenter.TriggerEvent(unLoadBundleCompleteEvent);
        }

        private bool CheckBundleNumber(string hashID)
        {
            if (!usedBundles.ContainsKey(hashID))
            {
                Debug.LogWarning($"unload bundle:{name}, that has never been used");
                return true;
            }

            int number = usedBundles[hashID];
            usedBundles[hashID] = number - 1;
            if (usedBundles[hashID] <= 0)
            {
                return true;
            }

            return false;
        }

        private void UnloadBundleForQueue(List<string> hashIDs, string manifestPath)
        {
            if (hashIDs == null || hashIDs.Count <= 0)
            {
                return;
            }

            foreach (var hashID in hashIDs)
            {
                bundleUnloadQueue.Enqueue(hashID);
            }

            bundleUnloadQueue.Enqueue(manifestPath + manifestMark);
        }

        private IEnumerator CheckBundleUnloadQueue()
        {
            while (checkQueueSwitch)
            {
                yield return new WaitForSeconds(10);
                bundleUnloadQueue.Enqueue(endMark);
                if (!CheckBundleCanBeUnload())
                {
                    continue;
                }
            }
        }

        private bool CheckBundleCanBeUnload()
        {
            if (bundleUnloadQueue.Count <= 0)
            {
                return false;
            }

            string hashID = bundleUnloadQueue.Peek();
            if (!usedBundles.ContainsKey(hashID) && Path.GetExtension(hashID) != endMark &&
                Path.GetExtension(hashID) != manifestMark)
            {
                bundleUnloadQueue.Dequeue();
                return CheckBundleCanBeUnload();
            }

            if (hashID == endMark)
            {
                bundleUnloadQueue.Dequeue();
                return true;
            }

            if (Path.GetExtension(hashID) == manifestMark)
            {
                bundleUnloadQueue.Dequeue();
                UnloadedBundleCallback(true, hashID);
                return CheckBundleCanBeUnload();
            }

            if (usedBundles[hashID] > 0)
            {
                bundleUnloadQueue.Dequeue();
                return CheckBundleCanBeUnload();
            }

            StartCoroutine(new BundleManifest().UnLoadBundle(GetBundle(hashID), hashID,
                (isDone) => UnloadedBundleCallback(isDone, hashID)));
            bundleUnloadQueue.Dequeue();
            return CheckBundleCanBeUnload();
        }

        private void UnloadedBundleCallback(bool isDone, string hashID)
        {
            if (!isDone)
            {
                Debug.LogError($"UnLoadBundle {hashID} error");
            }

            if (usedBundles.ContainsKey(hashID))
            {
                usedBundles.Remove(hashID);
            }

            if (allBundles.ContainsKey(hashID))
            {
                allBundles.Remove(hashID);
            }

            if (Path.GetExtension(hashID) == manifestMark)
            {
                string path = hashID.Remove(hashID.LastIndexOf("."));
                if (manifestMap.ContainsKey(path))
                {
                    manifestMap.Remove(path);
                }
            }

            Debug.Log($"UnLoadBundle {hashID} success");
        }

        private void LoadAsyncByList(MultipleLoadAsyncRequestEvent multipleLoadAsyncRequestEvent)
        {
            Dictionary<string, ManifestHandlerBase> tempManifests = new Dictionary<string, ManifestHandlerBase>();
            foreach (var loadName in multipleLoadAsyncRequestEvent.LoadNames)
            {
                ManifestHandlerBase iManifest = FindAssetByLoadName(loadName);
                if (iManifest == null)
                {
                    Debug.LogError($"Not find asset:{loadName}!");
                    return;
                }

                tempManifests.Add(loadName, iManifest);
            }

            switch (multipleLoadAsyncRequestEvent.ManifestType)
            {
                case ManifestTypes.Bundle:
                    List<LoadAssetData> loadBundleDatas = new List<LoadAssetData>();
                    List<LoadAssetData> loadBundleErrorDatas = new List<LoadAssetData>();
                    MultipleLoadAsyncBundleManifestsHandler(tempManifests, loadBundleDatas, loadBundleErrorDatas);

                    MultipleLoadAsyncCompleteEvent loadAsyncCompleteBundleEvent =
                        new MultipleLoadAsyncCompleteEvent(multipleLoadAsyncRequestEvent.RequestID, true,
                            loadBundleDatas, loadBundleErrorDatas);
                    EventCenter.TriggerEvent(loadAsyncCompleteBundleEvent);
                    break;

                case ManifestTypes.Resource:
                    List<LoadAssetData> loadResourceDatas = new List<LoadAssetData>();
                    List<LoadAssetData> loadResourceErrorDatas = new List<LoadAssetData>();
                    MultipleLoadAsyncResourceManifestsHandler(tempManifests, loadResourceDatas, loadResourceErrorDatas);

                    MultipleLoadAsyncCompleteEvent loadAsyncCompleteResourceEvent =
                        new MultipleLoadAsyncCompleteEvent(multipleLoadAsyncRequestEvent.RequestID, true,
                            loadResourceDatas, loadResourceErrorDatas);
                    EventCenter.TriggerEvent(loadAsyncCompleteResourceEvent);
                    break;

                case ManifestTypes.Scene:
                    List<LoadAssetData> loadSceneDatas = new List<LoadAssetData>();
                    List<LoadAssetData> loadSceneErrorDatas = new List<LoadAssetData>();
                    MultipleLoadAsyncResourceManifestsHandler(tempManifests, loadSceneDatas, loadSceneErrorDatas);

                    MultipleLoadAsyncCompleteEvent loadAsyncCompleteSceneEvent =
                        new MultipleLoadAsyncCompleteEvent(multipleLoadAsyncRequestEvent.RequestID, true,
                            loadSceneDatas, loadSceneErrorDatas);
                    EventCenter.TriggerEvent(loadAsyncCompleteSceneEvent);
                    break;
            }
        }

        private void LoadAsyncAllRequestHandle(LoadAsyncAllRequestEvent loadAsyncAllRequest)
        {
            ManifestHandlerBase iManifest = FindManifestToPath(loadAsyncAllRequest.ManifestPath);
            if (iManifest == null)
            {
                Debug.LogError("the manifest not loaded,please use 'LoadBundlesRequestEvent' to load.");
                return;
            }

            if (iManifest is ResourceManifest == false)
            {
                Debug.LogError("the manifest is not resource manifest,load async undone");
                return;
            }

            List<string> names = iManifest.GetAllHashID();
            WaitForLoadingBundles(names,
                () => ContinueLoadBundle(names, loadAsyncAllRequest.RequestID, iManifest as ResourceManifest, false));
        }

        private void WaitForLoadingBundles(List<string> names, Action callback)
        {
            StartCoroutine(WaitForLoadingBundlesAsync(names, callback));
        }

        private AssetBundle CheckBundle<T>(string name, T manifest) where T : ManifestHandlerBase
        {
            return GetBundle(manifest.FindHashIDToName(name));
        }

        private AssetBundle GetBundle(string hashID)
        {
            if (!allBundles.TryGetValue(hashID, out AssetBundle bundle))
            {
                Debug.LogError($"lost bundle!:{hashID}");
                return null;
            }

            return bundle;
        }

        private void LoadAsyncAllCompleteCallback(bool isDone, string message, GameObject prefab, Guid requestID)
        {
            LoadAsyncAllCompleteEvent loadAsyncAllCompleteEvent;
            if (!isDone)
            {
                Debug.LogError($"Load Async All error! :{message}");
                loadAsyncAllCompleteEvent = new LoadAsyncAllCompleteEvent(requestID, isDone, null);
                EventCenter.TriggerEvent(loadAsyncAllCompleteEvent);
                return;
            }

            loadAsyncAllCompleteEvent = new LoadAsyncAllCompleteEvent(requestID, isDone, prefab);
            EventCenter.TriggerEvent(loadAsyncAllCompleteEvent);
        }

        private void MultipleLoadAsyncBundleManifestsHandler(Dictionary<string, ManifestHandlerBase> tempManifests,
            List<LoadAssetData> loadAssetDatas, List<LoadAssetData> loadAssetErrorDatas)
        {
            foreach (var loadNameAndManifest in tempManifests)
            {
                BundleManifest bundleManifest = (BundleManifest)loadNameAndManifest.Value;
                if (bundleManifest.LoadAsync(CheckBundle(loadNameAndManifest.Key, bundleManifest),
                        loadNameAndManifest.Key, out Object bundleAsset))
                {
                    LoadAssetData loadAssetData = new LoadAssetData(loadNameAndManifest.Key, bundleAsset);
                    loadAssetDatas.Add(loadAssetData);
                }
                else
                {
                    LoadAssetData loadAssetData = new LoadAssetData(loadNameAndManifest.Key, bundleAsset);
                    loadAssetErrorDatas.Add(loadAssetData);
                }
            }
        }

        private void MultipleLoadAsyncResourceManifestsHandler(Dictionary<string, ManifestHandlerBase> tempManifests,
            List<LoadAssetData> loadAssetDatas, List<LoadAssetData> loadAssetErrorDatas)
        {
            foreach (var loadNameAndManifest in tempManifests)
            {
                ResourceManifest bundleManifest = (ResourceManifest)loadNameAndManifest.Value;
                if (bundleManifest.LoadAsync(CheckBundle(loadNameAndManifest.Key, bundleManifest),
                        loadNameAndManifest.Key,
                        out Object bundleAsset))
                {
                    LoadAssetData loadAssetData = new LoadAssetData(loadNameAndManifest.Key, bundleAsset);
                    loadAssetDatas.Add(loadAssetData);
                }
                else
                {
                    LoadAssetData loadAssetData = new LoadAssetData(loadNameAndManifest.Key, bundleAsset);
                    loadAssetErrorDatas.Add(loadAssetData);
                }
            }
        }

        private void MultipleLoadAsyncSceneManifestsHandler(Dictionary<string, ManifestHandlerBase> tempManifests,
            List<LoadAssetData> loadAssetDatas, List<LoadAssetData> loadAssetErrorDatas)
        {
            foreach (var loadNameAndManifest in tempManifests)
            {
                SceneManifest bundleManifest = (SceneManifest)loadNameAndManifest.Value;
                if (bundleManifest.LoadAsync(CheckBundle(loadNameAndManifest.Key, bundleManifest),
                        loadNameAndManifest.Key, out Object bundleAsset))
                {
                    LoadAssetData loadAssetData = new LoadAssetData(loadNameAndManifest.Key, bundleAsset);
                    loadAssetDatas.Add(loadAssetData);
                }
                else
                {
                    LoadAssetData loadAssetData = new LoadAssetData(loadNameAndManifest.Key, bundleAsset);
                    loadAssetErrorDatas.Add(loadAssetData);
                }
            }
        }

        private void LoadBundleHandle(LoadBundleRequestEvent loadBundleRequestEvent)
        {
            List<string> errorPaths = new List<string>();
            List<string> bundleNames = new List<string>();
            Dictionary<string, ManifestHandlerBase> bundleManifests = new Dictionary<string, ManifestHandlerBase>();
            foreach (var manifestPath in loadBundleRequestEvent.ManifestPaths)
            {
                ManifestHandlerBase iManifest = FindManifestToPath(manifestPath);
                if (iManifest == null)
                {
                    byte[] bs;
                    using (FileStream fs = File.Open(manifestPath, FileMode.Open))
                    {
                        bs = new byte[fs.Length];
                        fs.Read(bs, 0, bs.Length);
                        fs.Close();
                        fs.Dispose();
                    }

                    if (bs.Length == 0)
                    {
                        Debug.LogError("manifest is null or does not exist!");
                        errorPaths.Add(manifestPath);
                    }

                    string jsonData = Encoding.UTF8.GetString(bs);
                    iManifest = JsonUtility.FromJson<BundleManifest>(jsonData);
                }

                if (iManifest is BundleManifest bundleManifest)
                {
                    bundleNames.Add(bundleManifest.HashID);
                }

                bundleManifests.Add(manifestPath, iManifest);
            }

            BundleManifest tempManifest = new BundleManifest();
            StartCoroutine(tempManifest.LoadBundle(loadBundleRequestEvent.BundleFolderPath
                , (condition, errorPath) =>
                {
                    foreach (var variable in errorPath)
                    {
                        errorPaths.Add(variable);
                    }

                    LoadBundleCompleteCallbackHandler(loadBundleRequestEvent.RequestID, condition, errorPaths,
                        bundleManifests);
                }
                , bundleNames.ToArray()));
        }

        private void LoadBundleCompleteCallbackHandler(Guid requestID, bool result, List<string> errorPath,
            Dictionary<string, ManifestHandlerBase> bundleManifests)
        {
            foreach (var variable in bundleManifests)
            {
                if (!errorPath.Contains(variable.Key))
                {
                    manifestMap.Add(variable.Key, variable.Value);
                }
            }

            LoadBundleCompleteEvent completeEvent =
                new LoadBundleCompleteEvent(requestID, result, errorPath.ToArray());
            EventCenter.TriggerEvent(completeEvent);
        }

        private void UnLoadBundleHandle(UnLoadBundleRequestEvent unLoadBundleRequestEvent)
        {
            string unloadName = unLoadBundleRequestEvent.LoadName;
            ManifestHandlerBase iManifest = FindAssetByLoadName(unloadName);
            string path = GetPathByManifest(iManifest);
            if (path == "")
            {
                UnloadBundleCallback(unLoadBundleRequestEvent.RequestID, false, unloadName);
                return;
            }

            if (iManifest == null)
            {
                Debug.LogError($"Not find asset:{unloadName}!");
                UnloadBundleCallback(unLoadBundleRequestEvent.RequestID, false, unloadName);
                return;
            }

            string hashID = iManifest.FindHashIDToName(unloadName);
            if (!UnloadUseBundleByNameHandle(hashID, iManifest))
            {
                UnloadBundleCallback(unLoadBundleRequestEvent.RequestID, true, hashID);
                return;
            }

            UnloadBundleForQueue(new List<string>() { hashID }, path);
            UnloadBundleCallback(unLoadBundleRequestEvent.RequestID, true, hashID);
        }

        private string GetPathByManifest(ManifestHandlerBase iManifest)
        {
            string manifestPath = string.Empty;
            foreach (var variable in manifestMap)
            {
                if (variable.Value == iManifest)
                {
                    manifestPath = variable.Key;
                    break;
                }
            }

            if (string.IsNullOrEmpty(manifestPath))
            {
                Debug.LogError("Not Find Manifest Path!");
            }

            return manifestPath;
        }

        private void UnloadBundleCallback(Guid requestID, bool isdone, string unloadName)
        {
            if (!isdone)
            {
                Debug.LogError($"Unload bundle: {unloadName} error!");
            }

            UnLoadBundleCompleteEvent unLoadBundleCompleteEvent =
                new UnLoadBundleCompleteEvent(requestID, isdone);
            EventCenter.TriggerEvent(unLoadBundleCompleteEvent);
        }

        private bool UnloadUseBundleByNameHandle(string hashID, ManifestHandlerBase iManifest)
        {
            return CheckBundleNumber(hashID);
        }

        private void GetAllManifestHandle(GetAllManifestPathRequestEvent getAllManifestPathRequestEvent)
        {
            List<string> tempMap = new List<string>();
            foreach (var pair in manifestMap)
            {
                tempMap.Add(pair.Key);
            }

            GetAllManifestPathCompleteEvent getAllManifestPathCompleteEvent = null;
            if (tempMap.Count == 0)
            {
                getAllManifestPathCompleteEvent =
                    new GetAllManifestPathCompleteEvent(getAllManifestPathRequestEvent.RequestID, false, null);
                EventCenter.TriggerEvent(getAllManifestPathCompleteEvent);
            }

            getAllManifestPathCompleteEvent =
                new GetAllManifestPathCompleteEvent(getAllManifestPathRequestEvent.RequestID, true, tempMap);
            EventCenter.TriggerEvent(getAllManifestPathCompleteEvent);
        }

        private void WarmUpHandle(WarmUpForSceneRequestEvent warmUpForSceneRequestEvent)
        {
            SceneManifest sceneManifest =
                (SceneManifest)FindAssetByLoadName(warmUpForSceneRequestEvent.LoadName);
            if (!WarmUpForScene(warmUpForSceneRequestEvent.LoadName, sceneManifest))
            {
                WarmUpForSceneCompleteEvent warmUpForSceneCompleteEvent =
                    new WarmUpForSceneCompleteEvent(warmUpForSceneRequestEvent.RequestID, false);
                EventCenter.TriggerEvent(warmUpForSceneCompleteEvent);
                return;
            }

            WarmUpForSceneCompleteEvent completeEvent =
                new WarmUpForSceneCompleteEvent(warmUpForSceneRequestEvent.RequestID, true);
            EventCenter.TriggerEvent(completeEvent);
        }

        public bool WarmUpForScene(string sceneName, SceneManifest sceneManifest)
        {
            if (!allBundles.TryGetValue(sceneManifest.FindHashIDToName(sceneName), out AssetBundle bundle))
            {
                return false;
            }

            return true;
        }

        private ManifestHandlerBase FindAssetByLoadName(string loadName)
        {
            ManifestHandlerBase manifest = null;
            foreach (var manifests in manifestMap)
            {
                if (FindAssetToSceneManifest(manifests.Value, loadName, ref manifest))
                {
                    return manifest;
                }

                if (FindAssetToResourceManifest(manifests.Value, loadName, ref manifest))
                {
                    return manifest;
                }

                if (FindAssetToBundleManifest(manifests.Value, loadName, ref manifest))
                {
                    return manifest;
                }
            }

            return manifest;
        }

        private bool FindAssetToBundleManifest(ManifestHandlerBase manifestHandlerBase, string loadName,
            ref ManifestHandlerBase manifest)
        {
            if (CompareName((manifestHandlerBase as BundleManifest)?.ResourcePath, loadName))
            {
                manifest = manifestHandlerBase;
                return true;
            }

            return false;
        }

        private bool FindAssetToSceneManifest(ManifestHandlerBase manifestHandlerBase, string loadName,
            ref ManifestHandlerBase manifest)
        {
            List<SceneBlockData> sceneBlockDatas = (manifestHandlerBase as SceneManifest)?.SceneBlockMap;
            if (sceneBlockDatas == null)
            {
                return false;
            }

            foreach (var sceneBlockData in sceneBlockDatas)
            {
                if (CompareName(sceneBlockData.ScenePath, loadName))
                {
                    manifest = manifestHandlerBase;
                    return true;
                }
            }

            return false;
        }

        private bool FindAssetToResourceManifest(ManifestHandlerBase manifestHandlerBase, string loadName,
            ref ManifestHandlerBase manifest)
        {
            if ((manifestHandlerBase as ResourceManifest)?.FindHashIDToName(loadName) == null)
            {
                return false;
            }

            manifest = manifestHandlerBase;
            return true;
        }

        private bool CompareName(string assetName, string loadName)
        {
            if (assetName == loadName)
            {
                return true;
            }

            return false;
        }


        private void LoadAsyncHandle(LoadAsyncRequestEvent loadAsyncRequestEvent)
        {
            ManifestHandlerBase iManifest =
                FindAssetByLoadName(loadAsyncRequestEvent.LoadName);
            if (iManifest == null)
            {
                Debug.LogError($"Not find asset:{loadAsyncRequestEvent.LoadName}!");
                return;
            }

            string name = loadAsyncRequestEvent.LoadName;
            switch (loadAsyncRequestEvent.ManifestType)
            {
                case ManifestTypes.Bundle:
                    BundleManifest bundleManifest = (BundleManifest)iManifest;
                    bool resultBundle =
                        bundleManifest.LoadAsync(CheckBundle(name, bundleManifest), name, out Object bundleAsset);
                    LoadAsyncCompleteEvent loadAsyncCompleteBundleEvent =
                        new LoadAsyncCompleteEvent(loadAsyncRequestEvent.RequestID, resultBundle, null,
                            bundleAsset);
                    EventCenter.TriggerEvent(loadAsyncCompleteBundleEvent);
                    break;

                case ManifestTypes.Resource:
                    ResourceManifest resourceManifest = (ResourceManifest)iManifest;
                    bool resultResource =
                        resourceManifest.LoadAsync(CheckBundle(name, resourceManifest), name, out Object resourceAsset);
                    LoadAsyncCompleteEvent loadAsyncCompleteEvent =
                        new LoadAsyncCompleteEvent(loadAsyncRequestEvent.RequestID, resultResource, null,
                            resourceAsset);
                    EventCenter.TriggerEvent(loadAsyncCompleteEvent);
                    break;

                case ManifestTypes.Scene:
                    SceneManifest sceneManifest = (SceneManifest)iManifest;
                    bool resultScene =
                        sceneManifest.LoadAsync(CheckBundle(name, sceneManifest), name, out Object sceneAsset);

                    //TODO:trigger ture event to game manager
                    LoadAsyncCompleteEvent testTureManagerEvent =
                        new LoadAsyncCompleteEvent(loadAsyncRequestEvent.RequestID, resultScene, null,
                            sceneAsset);
                    EventCenter.TriggerEvent(testTureManagerEvent);
                    break;
            }
        }

        private void LoadManifestHandle(LoadBundlesRequestEvent bundlesRequestEventEvent)
        {
            ManifestHandlerBase iManifest = FindManifestToPath(bundlesRequestEventEvent.ManifestPath);

            string manifestPath = bundlesRequestEventEvent.ManifestPath;
            string bundleFolderPath = bundlesRequestEventEvent.BundleFolderPath;
            switch (bundlesRequestEventEvent.ManifestType)
            {
                case ManifestTypes.Bundle:
                    LoadBundleByManifest<BundleManifest>(iManifest, manifestPath, bundleFolderPath,
                        bundlesRequestEventEvent,
                        LoadBundlesCompleteCallback);
                    break;

                case ManifestTypes.Resource:
                    LoadBundleByManifest<ResourceManifest>(iManifest, manifestPath, bundleFolderPath,
                        bundlesRequestEventEvent,
                        LoadBundlesCompleteCallback);
                    break;

                case ManifestTypes.Scene:
                    LoadBundleByManifest<SceneManifest>(iManifest, manifestPath, bundleFolderPath,
                        bundlesRequestEventEvent,
                        LoadBundlesCompleteCallback);
                    break;
            }
        }


        private void LoadBundlesCompleteCallback(Guid requestID, bool result, IReadOnlyList<string> errorPaths,
            string manifestPath, ManifestHandlerBase manifestInstance, Dictionary<string, AssetBundle> loadedBundles)
        {
            SetLoadedBundles(loadedBundles, errorPaths);
            LoadBundlesCompleteEvent loadBundlesCompleteEvent =
                new LoadBundlesCompleteEvent(requestID, result, errorPaths);
            EventCenter.TriggerEvent(loadBundlesCompleteEvent);
        }


        private ManifestHandlerBase GetManifest<T>(ManifestHandlerBase iManifest,
            LoadBundlesRequestEvent bundlesRequestEventEvent) where T : ManifestHandlerBase
        {
            if (iManifest == null)
            {
                byte[] bs;
                using (FileStream fs = File.Open(bundlesRequestEventEvent.ManifestPath, FileMode.Open))
                {
                    bs = new byte[fs.Length];
                    fs.Read(bs, 0, bs.Length);
                    fs.Close();
                    fs.Dispose();
                }

                if (bs.Length == 0)
                {
                    Debug.LogError("manifest is null or does not exist!");
                    LoadBundlesCompleteEvent loadBundlesCompleteEvent =
                        new LoadBundlesCompleteEvent(bundlesRequestEventEvent.RequestID, false,
                            new List<string> { bundlesRequestEventEvent.ManifestPath });
                    EventCenter.TriggerEvent(loadBundlesCompleteEvent);
                }

                iManifest = JsonUtility.FromJson<T>(Encoding.UTF8.GetString(bs));
                manifestMap.Add(bundlesRequestEventEvent.ManifestPath, iManifest);
                return iManifest;
            }

            return null;
        }

        private List<string> FindLoadedID<T>(ManifestHandlerBase iManifest) where T : ManifestHandlerBase
        {
            List<string> tempID = ((T)iManifest).GetAllHashID();
            List<string> loadedID = new List<string>();

            foreach (var id in tempID)
            {
                if (allBundles.ContainsKey(id) || loadingKey.Contains(id))
                {
                    loadedID.Add(id);
                }
                else
                {
                    loadingKey.Add(id);
                }
            }

            return loadedID;
        }

        private ManifestHandlerBase FindManifestToPath(string manifestPath)
        {
            if (manifestMap.TryGetValue(manifestPath, out ManifestHandlerBase manifestHandlerBase))
            {
                return manifestHandlerBase;
            }

            return null;
        }

        private void LoadedBundlesAndLoadAsyncAllRequestCallback(bool isDone, string message, GameObject prefab,
            Guid requestID)
        {
            LoadBundlesAndAllAssetCompleteEvent loadBundlesAndAllAssetCompleteEvent;
            if (!isDone)
            {
                Debug.LogError($"Load Async All error! :{message}");
                loadBundlesAndAllAssetCompleteEvent = new LoadBundlesAndAllAssetCompleteEvent(requestID, isDone, null);
                EventCenter.TriggerEvent(loadBundlesAndAllAssetCompleteEvent);
                return;
            }

            loadBundlesAndAllAssetCompleteEvent = new LoadBundlesAndAllAssetCompleteEvent(requestID, isDone, prefab);
            EventCenter.TriggerEvent(loadBundlesAndAllAssetCompleteEvent);
        }

        private IEnumerator WaitForLoadingBundlesAsync(List<string> loadBundlesName, Action callback)
        {
            List<string> tempName = new List<string>();
            for (int i = 0; i < loadBundlesName.Count; i++)
            {
                if (loadingKey.Contains(loadBundlesName[i]))
                {
                    tempName.Add(loadBundlesName[i]);
                }
            }

            bool swith = true;
            while (swith)
            {
                for (int i = 0; i < tempName.Count; i++)
                {
                    if (!loadingKey.Contains(tempName[i]))
                    {
                        tempName.Remove(tempName[i]);
                    }
                }

                if (tempName.Count == 0)
                {
                    swith = false;
                }

                yield return null;
            }

            callback?.Invoke();
        }
    }
}