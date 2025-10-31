using Game.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffManager
{
    Canvas _canvas;
    GridLayoutGroup _layout;
    Dictionary<int, Buff> _buffDict = new Dictionary<int, Buff>();
    public void Init()
    {
        Manager.Resource.LoadAssetAsync<Canvas>("BuffCanvas", (canvas) =>
        {
            _canvas = GameObject.Instantiate(canvas);
            Object.DontDestroyOnLoad(_canvas.gameObject);
            _layout = _canvas.transform.FindChild<GridLayoutGroup>();

            Manager.Resource.LoadAssetAsync<Buff>("BuffUI", (buffPrefab) =>
            {
                var buffDict = Manager.Data.ItemsData.Buff;
                foreach (var buffData in buffDict)
                {
                    var buffUI = GameObject.Instantiate(buffPrefab);
                    buffUI.transform.parent = _layout.transform;
                    buffUI.Init(buffData.Value);
                    _buffDict.Add(buffData.Key, buffUI);
                }
            });
        });
    }
    public void OnBuff(ItemBuffData data, PlayerCharacter player)
    {
        Buff buff = _buffDict[data.Id];
        buff.ResetTime();
        bool last = false;
        if (buff.BuffCoroutine == null)
        {
            last = true;
        }
        else
            buff.CountBuff(player);

        buff.OnBuff(player, last);
    }
    public void Clear()
    {
        foreach (var buffData in _buffDict)
        {
            if (buffData.Value.BuffCoroutine != null)
            {
                PlayerCharacter.Instance.StopCoroutine(buffData.Value.BuffCoroutine);
                buffData.Value.BuffCoroutine = null;
            }
        }
    }
    public void Release()
    {
        Clear();
        if (_canvas != null)
        {
            Object.Destroy(_canvas.gameObject);
            _canvas = null;
        }
    }
}
