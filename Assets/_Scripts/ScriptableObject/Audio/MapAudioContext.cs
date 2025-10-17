using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IAudioDataContext
{
    public Dictionary<string, AudioData> ToDict();
}

[Serializable]
public class PlayerAudioDataContext : IAudioDataContext
{
    public List<PlayerHitAudioData> PlayerAttackAudioDatas = new List<PlayerHitAudioData>();
    public List<PlayerMoveAudioData> PlayerMoveAudioDatas = new List<PlayerMoveAudioData>();
    public List<PlayerSkillAudioData> PlayerSkillAudioDatas = new List<PlayerSkillAudioData>();

    public Dictionary<string, AudioData> ToDict()
    {
        Dictionary<string, AudioData> dict = new Dictionary<string, AudioData>();
        PlayerAttackAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        PlayerMoveAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        PlayerSkillAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        return dict;
    }
}

[Serializable]
public class MonsterAudioDataContext : IAudioDataContext
{
    public List<MonsterAttackAudioData> MonsterAttackAudioDatas = new List<MonsterAttackAudioData>();
    public List<MonsterDieAudioData> MonsterDieAudioDatas = new List<MonsterDieAudioData>();
    public List<MonsterProjectileAudioData> MonsterProjectileAudioDatas = new List<MonsterProjectileAudioData>();

    public Dictionary<string, AudioData> ToDict()
    {
        Dictionary<string, AudioData> dict = new Dictionary<string, AudioData>();
        MonsterAttackAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        MonsterDieAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        MonsterProjectileAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        return dict;
    }
}

[Serializable]
public class ItemAudioDataContext : IAudioDataContext
{
    public List<ItemBoxAudioData> itemBoxAudioDatas = new List<ItemBoxAudioData>();
    public List<ItemEffectAudioData> itemEffectAudioDatas = new List<ItemEffectAudioData>();

    public Dictionary<string, AudioData> ToDict()
    {
        Dictionary<string, AudioData> dict = new Dictionary<string, AudioData>();
        itemBoxAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        itemEffectAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        return dict;
    }
}

[Serializable]
public class EnvironmentAudioDataContext : IAudioDataContext
{
    public List<EnvironmentAudioData> environmentAudioDatas = new List<EnvironmentAudioData>();

    public Dictionary<string, AudioData> ToDict()
    {
        Dictionary<string, AudioData> dict = new Dictionary<string, AudioData>();
        environmentAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        return dict;
    }
}

[Serializable]
public class BGMAudioDataContext : IAudioDataContext
{
    [SerializeField]
    public List<BGMAudioData> bgmAudioDatas = new List<BGMAudioData>();

    public Dictionary<string, AudioData> ToDict()
    {
        Dictionary<string, AudioData> dict = new Dictionary<string, AudioData>();
        bgmAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        return dict;
    }
}

[Serializable]
public class DirecitonAudioDataContext : IAudioDataContext
{
    public List<DirectionAudioData> directionalAudioDatas = new List<DirectionAudioData>();

    public Dictionary<string, AudioData> ToDict()
    {
        Dictionary<string, AudioData> dict = new Dictionary<string, AudioData>();
        directionalAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        return dict;
    }
}

[Serializable]
public class UIAudioDataContext : IAudioDataContext
{
    public List<UIAudioData> uiAudioDatas = new List<UIAudioData>();

    public Dictionary<string, AudioData> ToDict()
    {
        Dictionary<string, AudioData> dict = new Dictionary<string, AudioData>();
        uiAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        return dict;
    }
}

[Serializable]
public class TraitAudioDataContext : IAudioDataContext
{
    public List<TraitAudioData> traitAudioDatas = new List<TraitAudioData>();

    public Dictionary<string, AudioData> ToDict()
    {
        Dictionary<string, AudioData> dict = new Dictionary<string, AudioData>();
        traitAudioDatas.ForEach(x => dict.Add(x.AudioKey.ToString(), x));
        return dict;
    }
}

[CreateAssetMenu(fileName = "New Map Audio Context", menuName = "ScriptableObject/Audio/Map/Map Audio Context", order = 0)]
public class MapAudioContext : ScriptableObject, IAudioDataContext
{
    public PlayerAudioDataContext PlayerAudioDataContext;
    public MonsterAudioDataContext MonsterAudioDataContext;
    public ItemAudioDataContext ItemAudioDataContext;
    public EnvironmentAudioDataContext EnvironmentAudioDataContext;
    public BGMAudioDataContext BGMAudioDataContext;
    public DirecitonAudioDataContext DirecitonAudioDataContext;
    public UIAudioDataContext UIAudioDataContext;
    public TraitAudioDataContext TraitAudioDataContext;

    public bool AutoSetBGM;

    public Dictionary<string, AudioData> ToDict()
    {
        Dictionary<string, AudioData> dict = new Dictionary<string, AudioData>();
        PlayerAudioDataContext?.ToDict().ToList().ForEach(x => dict.Add(x.Key, x.Value));
        MonsterAudioDataContext?.ToDict().ToList().ForEach(x => dict.Add(x.Key, x.Value));
        ItemAudioDataContext?.ToDict().ToList().ForEach(x => dict.Add(x.Key, x.Value));
        EnvironmentAudioDataContext?.ToDict().ToList().ForEach(x => dict.Add(x.Key, x.Value));
        BGMAudioDataContext?.ToDict().ToList().ForEach(x => dict.Add(x.Key, x.Value));
        DirecitonAudioDataContext?.ToDict().ToList().ForEach(x => dict.Add(x.Key, x.Value));
        UIAudioDataContext?.ToDict().ToList().ForEach(x => dict.Add(x.Key, x.Value));
        TraitAudioDataContext?.ToDict().ToList().ForEach(x => dict.Add(x.Key, x.Value));
        return dict;
    }
}
