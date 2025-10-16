using UnityEngine;

public static class AudioKey
{
    public static class Player
    {
        public enum Move
        {
            WALK,
            DASH,
            JUMP_START,
            JUMP_END,
            TAKE_DAMAGE,
        }
        public enum Hit
        {
            MISS,
            UNDEAD_BOSS,
            UNDEAD_SHIELD,
            SLIME,
            GOLEM,
        }
        public enum Skill
        {
            SHADOW_CREATE,
            SHADOW_HIT,
            WIND_SWING,
            HIT_EXTRA_AWAKEN,
        }
    }
    public static class Monster
    {
        public enum Die
        {
            SLIME,
            UNDEAD,
            GOLEM,
            BOSS,
        }
        public enum Attack
        {
            UNDEAD_CLOSE,
            UNDEAD_MAGE,
            GOLEM,
            BOSS_HAND_DOWN,
            BOSS_LASER,
            BOSS_ORB_PHASE1,
            BOSS_ORB_PHASE2,
            BOSS_BREATH,
        }
        public enum Projectile
        {
            UNDEAD_MAGE_CREATE,
            UNDEAD_MAGE_SHOT,
        }
    }
    public static class Item
    {
        public enum Box
        {
            BOX_SHAKE,
            BOX_OPEN,
            LEGENDARY_ITEM,
            COLLECT_ITEM,
        }
        public enum Effect
        {
            WIND,
            THUNDER,
            HOWLING,
        }
    }
    public enum Direction
    {
        BOSS_START,
        STAGE_CLEAR,
        DEATH_SCREEN_DARKEN,
        DEATH_SCREEN_LIGHTEN,
        DEATH_TEXT,
        PLAYER_SPAWN,
    }
    public enum BGM
    {
        BASE,
        BOSS_PHASE1,
        BOSS_PHASE2,
    }
    public enum UI
    {
        CLICK,
        TITLE
    }
    public enum Trait
    {
        NPC_INTERACT,
        SUCCESS,
        FAIL,
    }
    public enum Environment
    {
        PORTAL,
        GOLD_DESTROY,
        GOLD_COLLECT,
    }
}
public class AudioData : ScriptableObject
{
    [SerializeField]
    AudioClip Clip;
    public AudioClip AudioClip { get { return Clip; } }
    [SerializeField]
    bool Loop = false;
    public bool IsLoop { get { return Loop; } }
}
