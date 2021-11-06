using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetRefLoader
{
    public static Dictionary<string, AsyncOperationHandle<GameObject>> OperationHandleDict = new Dictionary<string, AsyncOperationHandle<GameObject>>();
    public static async Task GetAll(string assetLabelOrName, IList<IResourceLocation> loadedLocations)
    {
        var unloadedLocations = await Addressables.LoadResourceLocationsAsync(assetLabelOrName).Task;
        foreach (var location in unloadedLocations)
        {
            loadedLocations.Add(location);
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~ Load Assets ~~~~~~~~~~~~~~~~~~

    public static async Task LoadedAssets<T>(string assetLabelOrName, Action<T> callback) where T : UnityEngine.Object
    {
        T GO = await Addressables.LoadAssetsAsync(assetLabelOrName, callback).Task as T;
    }

    public static async Task LoadedAssets<T>(AssetLabelReference label, Action<T> callback) where T : UnityEngine.Object
    {
        if (label.RuntimeKeyIsValid())
        {
            T GO = await Addressables.LoadAssetsAsync(label.labelString, callback).Task as T;
        }       
    }

    //~~~~~~~~~~~~~~~~~~~~~~~ Create Asset ~~~~~~~~~~~~~~~~~~~~~~~~~

    public static async Task CreatedAsset<T>(string assetLabelOrName, T createdObjs) where T : UnityEngine.Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(assetLabelOrName).Task;
        foreach (var location in locations)
        {
            createdObjs = await Addressables.InstantiateAsync(location).Task as T;           
        }
    }

    public static async Task CreatedAsset<T>(string assetLabelOrName, T createdObjs, Transform Parent) where T : UnityEngine.Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(assetLabelOrName).Task;
        foreach (var location in locations)
        {
            createdObjs = await Addressables.InstantiateAsync(location, Parent).Task as T;
        }
    }

    public static async Task CreatedAsset<T>(string assetLabelOrName, T createdObjs, Vector3 Position) where T : UnityEngine.Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(assetLabelOrName).Task;
        foreach (var location in locations)
        {
            createdObjs = await Addressables.InstantiateAsync(location, Position, Quaternion.identity).Task as T;
        }
    }

    public static async Task CreatedAsset<T>(string assetLabelOrName, T createdObjs, Transform Parent, Vector3 Position) where T : UnityEngine.Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(assetLabelOrName).Task;
        foreach (var location in locations)
        {
            createdObjs = await Addressables.InstantiateAsync(location, Position, Quaternion.identity, Parent).Task as T;
        }
    }

    public static async Task CreateAsset(string assetName, Vector3 Position)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(assetName, Position, Quaternion.identity);
        await handle.Task;
        if (handle.IsDone)
        {
            OperationHandleDict.Add(assetName, handle);
        }
    }

    //~~~~~~~~~~~~~~~~~~~ Create Gameobject And Release After Delay ~~~~~~~~~~~~~~~~~

    public static async void CreateAndReleaseAsset(string assetLabelOrName, Vector3 Position, float delayTime)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(assetLabelOrName, Position, Quaternion.identity);
        await handle.Task;
        if (handle.IsDone)
        {
            int delayInMilli = Mathf.FloorToInt(delayTime * 1000);
            await Task.Delay(delayInMilli);
            Addressables.ReleaseInstance(handle);
        }
    }

    public static async Task CreateAndReleaseAsset(string assetLabelOrName, GameObject createdObj, Vector3 Position, float delayTime)
    {
        var locations = await Addressables.LoadResourceLocationsAsync(assetLabelOrName).Task;
        foreach (var location in locations)
        {
            createdObj = await Addressables.InstantiateAsync(location, Position, Quaternion.identity).Task as GameObject;
            int delayInMilli = Mathf.FloorToInt(delayTime * 1000);
            await Task.Delay(delayInMilli);
            Addressables.Release(createdObj);
            //Addressables.ReleaseInstance(createdObj);
        }
    }

    //~~~~~~~~~~~~~~~~~~~~ Create Assets And Add To List ~~~~~~~~~~~~~~~~~~~~~

    public static async Task CreatedAssetsAddToList<T>(string assetLabelOrName, List<T> createdObjs) where T : UnityEngine.Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(assetLabelOrName).Task;
        foreach (var location in locations)
        {
            T GO = await Addressables.InstantiateAsync(location).Task as T;
            createdObjs.Add(GO);
        }
    }

    public static async Task CreatedAssetsAddToList<T>(string assetLabelOrName, List<T> createdObjs, Transform Parent) where T : UnityEngine.Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(assetLabelOrName).Task;
        foreach (var location in locations)
        {
            T GO = await Addressables.InstantiateAsync(location, Parent).Task as T;
            createdObjs.Add(GO);
        }
    }

    public static async Task CreatedAssetsAddToList<T>(List<string> assetLabelsOrNames, Dictionary<string, List<T>> createdObjs) where T : UnityEngine.Object
    {
        foreach (var asset in assetLabelsOrNames)
        {
            
            T GO = await Addressables.InstantiateAsync(asset).Task as T;                        
            if (!createdObjs.ContainsKey(asset))
            {
                List<T> tempList = new List<T>();
                tempList.Add(GO);
                createdObjs.Add(asset, tempList);
            }
            else
            {
                if (createdObjs.TryGetValue(asset, out List<T> value))
                    value.Add(GO);
            }
        }
    }

    public static async Task CreatedAssetsAddToList<T>(AssetLabelReference label, List<T> createdObjs) where T : UnityEngine.Object
    {
        if (label.RuntimeKeyIsValid())
        {
            var locations = await Addressables.LoadResourceLocationsAsync(label.labelString).Task;
            foreach (var location in locations)
            {
                T GO = await Addressables.InstantiateAsync(location).Task as T;
                createdObjs.Add(GO);
            }
        }
        
    }

    public static async Task CreatedAssetsAddToList<T>(AssetLabelReference label, List<T> createdObjs, Transform Parent) where T : UnityEngine.Object
    {
        if (label.RuntimeKeyIsValid())
        {
            var locations = await Addressables.LoadResourceLocationsAsync(label.labelString).Task;
            foreach (var location in locations)
            {
                T GO = await Addressables.InstantiateAsync(location, Parent).Task as T;
                createdObjs.Add(GO);
            }
        }

    }

    public static async Task CreatedAssetsAddToList<T>(List<AssetLabelReference> labels, Dictionary<AssetLabelReference, List<T>> createdObjs) where T : UnityEngine.Object
    {
        foreach (var label in labels)
        {
            if (label.RuntimeKeyIsValid())
            {
                T GO = await Addressables.InstantiateAsync(label.labelString).Task as T;
                if (!createdObjs.ContainsKey(label))
                {
                    List<T> tempList = new List<T>();
                    tempList.Add(GO);
                    createdObjs.Add(label, tempList);
                }
                else
                {
                    if (createdObjs.TryGetValue(label, out List<T> value))
                        value.Add(GO);
                }
            }            
        }
    }

    //~~~~~~~~~~~~~~~ Release Asset ~~~~~~~~~~~~~~

    public static async Task ReleaseAssetInstance(GameObject gameObject, float delayTime)
    {
        int delayInMilli = Mathf.FloorToInt(delayTime * 1000);
        await Task.Delay(delayInMilli);
        Addressables.ReleaseInstance(gameObject);
    }

    public static async void ReleaseAssetInstance(string assetName, float delayTime)
    {
        int delayInMilli = Mathf.FloorToInt(delayTime * 1000);
        await Task.Delay(delayInMilli);
        if (OperationHandleDict.TryGetValue(assetName, out AsyncOperationHandle<GameObject> handle))
        {
            Addressables.ReleaseInstance(handle);
            OperationHandleDict.Remove(assetName);
        }        
    }

    public static async Task ReleaseAssetInstance(GameObject gameObject, float delayTime, bool IsReleaseCompletely)
    {
        int delayInMilli = Mathf.FloorToInt(delayTime * 1000);
        await Task.Delay(delayInMilli);
        Addressables.ReleaseInstance(gameObject);
        if (IsReleaseCompletely)
            Addressables.Release(gameObject);
    }

    //~~~~~~~~~~ Using AssetReference ~~~~~~~~~~~

    public static async Task CreatedAssetsAddToList<T>(AssetReference reference, List<T> completedObjs, Vector3 position) where T : UnityEngine.Object
    {
        T GO = await reference.InstantiateAsync(position, Quaternion.identity).Task as T;
        completedObjs.Add(GO);
    }

    public static async Task CreatedAssetsAddToList<T>(AssetReference reference, List<T> completedObjs, Transform Parent) where T : UnityEngine.Object
    {
        T GO = await reference.InstantiateAsync(Parent).Task as T;
        completedObjs.Add(GO);
    }

    public static async Task CreatedAssetsAddToList<T>(AssetReference reference, List<T> completedObjs) where T : UnityEngine.Object
    {
        T GO = await reference.InstantiateAsync().Task as T;
        completedObjs.Add(GO);
    }

    public static async Task CreatedAssetsAddToList<T>(List<AssetReference> references, List<T> completedObjs) where T : UnityEngine.Object
    {
        foreach (var reference in references)
        {
            T GO = await reference.InstantiateAsync().Task as T;
            completedObjs.Add(GO);
        }
    }
}
