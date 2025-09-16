using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class ResourceManager
{
    /// <summary>
    /// Component 타입 비동기 로드
    /// </summary>
    /// <typeparam name="T">컴포넌트 타입</typeparam>
    /// <param name="prefabName">프리팹 이름</param>
    /// <param name="callback">완료 콜백</param>
    public void LoadComponentAsync<T>(string prefabName, Action<Component> callback) where T : Component
    {
        var oper = Addressables.LoadAssetAsync<GameObject>(prefabName);
        oper.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                callback?.Invoke(handle.Result.GetComponent<T>());
            }
            else
            {
                Debug.LogError($"Failed to load asset: {prefabName}");
                callback?.Invoke(null);
            }
            Addressables.Release(handle);
        };
    }

    /// <summary>
    /// ScriptableObject 타입 비동기 로드
    /// </summary>
    /// <typeparam name="T">데이터 타입</typeparam>
    /// <param name="name">이름</param>
    /// <param name="callback">완료 콜백</param>
    public void LoadDataAsync<T>(string name, Action<ScriptableObject> callback) where T : ScriptableObject
    {
        var oper = Addressables.LoadAssetAsync<ScriptableObject>(name);
        oper.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                callback?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"Failed to load asset: {name}");
                callback?.Invoke(null);
            }
            Addressables.Release(handle);
        };
    }
    /// <summary>
    /// 단일 에셋 비동기 로드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    public void LoadAssetAsync<T>(string name, Action<T> callback) where T : Object
    {
        var oper = Addressables.LoadAssetAsync<T>(name);
        oper.Completed += handle =>
        {
            callback?.Invoke(handle.Result);
            Addressables.Release(handle);
        };
    }

    /// <summary>
    /// 라벨에 해당하는 모든 에셋 비동기 로드
    /// </summary>
    /// <param name="labelName">라벨 이름</param>
    /// <param name="callback">완료 콜백</param>
    public void LoadAsync(string labelName, Action<List<Object>> callback)
    {
        var oper = Addressables.LoadAssetsAsync<Object>(labelName);

        oper.Completed += (handle) =>
        {
            List<Object> list = new List<Object>(handle.Result);
            callback(list);
            Addressables.Release(handle);
        };
    }
    /// <summary>
    /// Component 타입 라벨에 해당하는 모든 에셋 비동기 로드
    /// </summary>
    /// <typeparam name="T">컴포넌트 타입</typeparam>
    /// <param name="labelName">라벨 이름</param>
    /// <param name="callback">완료 콜백</param>
    public void LoadAsyncComponent<T>(string labelName, Action<List<Component>> callback) where T : Component
    {
        var oper = Addressables.LoadAssetsAsync<GameObject>(labelName);
        oper.Completed += (handle) =>
        {
            List<Component> list = new List<Component>();
            foreach (var obj in handle.Result)
            {
                var component = obj.GetComponent<T>();
                if (component != null)
                {
                    list.Add(component);
                }
            }
            callback(list);
            Addressables.Release(handle);
        };
    }
    /// <summary>
    /// ScriptableObject 타입 라벨에 해당하는 모든 에셋 비동기 로드
    /// </summary>
    /// <typeparam name="T">데이터 타입</typeparam>
    /// <param name="labelName">라벨 이름</param>
    /// <param name="callback">완료 콜백</param>
    public void LoadDataAsync<T>(string labelName, Action<List<ScriptableObject>> callback) where T : ScriptableObject
    {
        var oper = Addressables.LoadAssetsAsync<ScriptableObject>(labelName);
        oper.Completed += (handle) =>
        {
            List<ScriptableObject> list = new List<ScriptableObject>(handle.Result);
            callback(list);
            Addressables.Release(handle);
        };
    }
    /// <summary>
    /// Component 타입 동기 로드
    /// </summary>
    /// <typeparam name="T">컴포넌트 타입</typeparam>
    /// <param name="prefabName">프리팹 이름</param>
    /// <returns>컴포넌트 T</returns>
    public T LoadComponent<T>(string prefabName) where T : Component
    {
        var result = Addressables.LoadAssetAsync<GameObject>(prefabName).WaitForCompletion();
        if (result != null)
            return result.GetComponent<T>();
        return null;
    }
    /// <summary>
    /// ScriptableObject 타입 동기 로드
    /// </summary>
    /// <typeparam name="T">데이터 타입</typeparam>
    /// <param name="name">이름</param>
    /// <returns>데이터 T</returns>
    public T LoadData<T>(string name) where T : ScriptableObject
    {
        var result = Addressables.LoadAssetAsync<ScriptableObject>(name).WaitForCompletion();
        if (result != null)
            return result as T;
        return null;
    }
    /// <summary>
    /// 단일 에셋 동기 로드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T Load<T>(string name) where T : Object
    {
        var result = Addressables.LoadAssetAsync<T>(name).WaitForCompletion();
        if (result != null)
            return result;
        return null;
    }
    /// <summary>
    /// Component 타입 라벨에 해당하는 모든 에셋 동기 로드
    /// </summary>
    /// <typeparam name="T">컴포넌트 타입</typeparam>
    /// <param name="labelName">라벨 이름</param>
    /// <returns>라벨에 해당하는 컴포넌트 List<T></returns>
    public List<T> LoadAllComponent<T>(string labelName) where T : Component
    {
        var result = Addressables.LoadAssetsAsync<GameObject>(labelName).WaitForCompletion();
        if (result != null)
        {
            List<T> list = new List<T>();
            foreach (var obj in result)
            {
                var component = obj.GetComponent<T>();
                if (component != null)
                {
                    list.Add(component);
                }
            }
            return list;
        }
        return null;
    }
    /// <summary>
    /// ScriptableObject 타입 라벨에 해당하는 모든 에셋 동기 로드
    /// </summary>
    /// <typeparam name="T">데이터 타입</typeparam>
    /// <param name="labelName">라벨 이름</param>
    /// <returns>라벨에 해당하는 데이터 List<T></returns>
    public List<T> LoadAllData<T>(string labelName) where T : ScriptableObject
    {
        var result = Addressables.LoadAssetsAsync<ScriptableObject>(labelName).WaitForCompletion();
        if (result != null)
        {
            List<T> list = new List<T>();
            foreach (var obj in result)
            {
                if (obj is T data)
                {
                    list.Add(data);
                }
            }
            return list;
        }
        return null;
    }
    /// <summary>
    /// 라벨에 해당하는 모든 에셋 동기 로드
    /// </summary>
    /// <param name="labelName">라벨 이름</param>
    /// <returns></returns>
    public List<Object> LoadAll<Object>(string labelName)
    {
        var result = Addressables.LoadAssetsAsync<Object>(labelName).WaitForCompletion();
        if (result != null)
            return new List<Object>(result);
        return null;
    }
    public T Instantiate<T>(T prefab) where T : Object
    {
        return Object.Instantiate<T>(prefab);
    }
    public void Destroy(Object obj)
    {
        Object.Destroy(obj);
    }
}
