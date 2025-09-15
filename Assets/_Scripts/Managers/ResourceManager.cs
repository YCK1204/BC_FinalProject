using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class ResourceManager
{
    /// <summary>
    /// 단일 에셋 비동기 로드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="labelName">라벨 이름</param>
    /// <param name="prefabName">프리팹 이름</param>
    /// <param name="callback">완료 콜백</param>
    public void LoadAsync<T>(string labelName, string prefabName, Action<T> callback) where T : Object
    {
        var oper = Addressables.LoadAssetsAsync<T>(labelName, null);

        oper.Completed += handle =>
        {
            foreach (var item in handle.Result)
            {
                if (item.name == prefabName)
                {
                    callback(item);
                    break;
                }
            }
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
        };
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
