using Game.Player;
using Newtonsoft.Json;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager
{
    public Item ItemsData = new Item();
    Dictionary<int, T1> MakeDict<T1, T2>(string fileName, Func<T2, Dictionary<int, T1>> factory) where T2 : class
    {
        try
        {
            var text = Manager.Resource.Load<TextAsset>(fileName);
            var wrapper = JsonConvert.DeserializeObject<T2>(text.text);
            return factory(wrapper);
        }
        catch (Exception e)
        {
            Debug.LogError($"DataManager Load Failed to {fileName}");
        }
        return new Dictionary<int, T1>();
    }
    // 절대 경로의 json 파일 역직렬화
    T MakeData<T>(string fileName) where T : class, new()
    {
        T result = null;
        try
        {
            var text = Manager.Resource.Load<TextAsset>(fileName);
            result = JsonConvert.DeserializeObject<T>(text.text);
        }
        catch (Exception e)
        {
            Debug.LogError($"DataManager Load Failed to {fileName}");
        }
        if (result == null)
            result = new T();
        return result;
    }
    // Resources 기준 path상대 경로에 있는 모든 파일을 Dictionary<int, T>로 변환하여 반환
    Dictionary<int, T> MakeScriptableObjectDict<T>(string labelName, Func<T, int> extractIdentifier) where T : ScriptableObject
    {
        Dictionary<int, T> dict = new Dictionary<int, T>();

        var allData = Manager.Resource.LoadAll<T>(labelName);

        foreach (var data in allData)
        {
            var id = extractIdentifier(data);
            if (!dict.TryAdd(id, data))
                Debug.LogError($"Overlapped Quest Data {data.name}");
        }
        return dict;
    }
    public void Load()
    {
        #region Item
        {
            var reader = Manager.Resource.LoadData<ItemDataReader>("ItemDataReader");
            ItemsData.Base = reader.DataList.ToDictionary(data => data.Id, data => data);
        }
        {
            var reader = Manager.Resource.LoadData<ItemBuffDataReader>("BuffDataReader");
            ItemsData.Buff = reader.DataList.ToDictionary(data => data.Id, data => data);
        }
        {
            var reader = Manager.Resource.LoadData<ItemProjectileDataReader>("ProjectileDataReader");
            ItemsData.Projectile = reader.DataList.ToDictionary(data => data.Id, data => data);
        }
        {
            var reader = Manager.Resource.LoadData<ItemAreaDataReader>("AreaDataReader");
            ItemsData.Area = reader.DataList.ToDictionary(data => data.Id, data => data);
        }
        {
            var reader = Manager.Resource.LoadData<ItemSynergyDataReader>("SynergyDataReader");
            ItemsData.Synergy = reader.DataList.ToDictionary(data => data.Id, data => data);
        }
        {
            var reader = Manager.Resource.LoadData<ItemDescriptionDataReader>("DescriptionDataReader");
            ItemsData.Description = reader.DataList.ToDictionary(data => data.Id, data => data);
        }
        #endregion
    }
    public void Save()
    {
    }
}