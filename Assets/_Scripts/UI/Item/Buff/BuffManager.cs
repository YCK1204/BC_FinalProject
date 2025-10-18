using DG.Tweening;
using Game.Player;
using Game.Traits.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Buff
{
    float _elapsedTime;
    public Coroutine BuffCoroutine;
    ItemBuffData _data;
    BuffUIContainer _container;
    public Buff(ItemBuffData data)
    {
        _data = data;
        _elapsedTime = 0f;
        BuffCoroutine = null;
        var container = Manager.Resource.Load<BuffUIContainer>("BuffUI");
        _container = GameObject.Instantiate(container);
        _container.gameObject.SetActive(false);
        _container.Set(_data);
    }
    public bool Elapse()
    {
        _elapsedTime += Time.deltaTime;
        float amount = Mathf.Clamp01(_elapsedTime / _data.Duration);
        _container.SetFillAmount(amount);
        if (amount >= 1f)
        {
            var player = PlayerCharacter.Instance;
            _container.gameObject.SetActive(false);
            ItemSetterUtil.RemoveStat(player, _data.Stat1.ItemExtraStatType, _data.Stat1.Value);
            ItemSetterUtil.RemoveStat(player, _data.Stat2.ItemExtraStatType, _data.Stat2.Value);
            return false;
        }
        return true;
    }
    public void ResetTime()
    {
        _elapsedTime = 0f;
    }
    public void OnBuff(PlayerCharacter player, bool last)
    {
        if (last)
            _container.transform.SetAsLastSibling();
        _container.gameObject.SetActive(true);
        ItemSetterUtil.ApplyStat(player, _data.Stat1.ItemExtraStatType, _data.Stat1.Value);
        ItemSetterUtil.ApplyStat(player, _data.Stat2.ItemExtraStatType, _data.Stat2.Value);
    }
    public void SetParent(Transform parent)
    {
        _container.transform.SetParent(parent);
    }
}
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

            var buffDict = Manager.Data.ItemsData.Buff;
            foreach (var buffData in buffDict)
            {
                Buff buff = new Buff(buffData.Value);
                buff.SetParent(_layout.transform);
                _buffDict.Add(buffData.Key, buff);
            }
        });
    }
    public void OnBuff(ItemBuffData data, PlayerCharacter player)
    {
        Buff buff = _buffDict[data.Id];
        buff.ResetTime();
        bool last = false;
        if (buff.BuffCoroutine != null)
        {
            ItemSetterUtil.RemoveStat(player, data.Stat1.ItemExtraStatType, data.Stat1.Value);
            ItemSetterUtil.RemoveStat(player, data.Stat2.ItemExtraStatType, data.Stat2.Value);
            player.StopCoroutine(buff.BuffCoroutine);
        }
        else
            last = true;
        buff.BuffCoroutine = player.StartCoroutine(CoBuff(buff, player, last));
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
    IEnumerator CoBuff(Buff buff, PlayerCharacter player, bool last)
    {
        buff.OnBuff(player, last);

        while (true)
        {
            if (buff.Elapse() == false)
                break;
            yield return null;
        }
        buff.BuffCoroutine = null;
    }
}
