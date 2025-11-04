using UnityEngine;

public static class AudioKey
{
    public static class Player
    {
        public enum Move
        {
            P_MOVE_WALK,
            P_MOVE_DASH,
            P_MOVE_JUMP_START,
            P_MOVE_JUMP_END,
            P_MOVE_TAKE_DAMAGE,
        }
        public enum Hit
        {
            P_HIT_MISS,
            P_HIT_UNDEAD_BOSS,
            P_HIT_UNDEAD_SHIELD,
            P_HIT_SLIME,
            P_HIT_GOLEM,
        }
        public enum Skill
        {
            P_SKILL_SHADOW_CREATE,
            P_SKILL_SHADOW_HIT,
            P_SKILL_WIND_SWING,
            P_SKILL_HIT_EXTRA_AWAKEN,
            P_SKILL_SWORD1,
            P_SKILL_SWORD2,
            P_SKILL_SWORD3,
            P_SKILL_SWORD4,
        }
    }
    public static class Monster
    {
        public enum Die
        {
            M_DIE_SLIME,
            M_DIE_UNDEAD,
            M_DIE_GOLEM,
            M_DIE_BOSS,
        }
        public enum Attack
        {
            M_ATK_UNDEAD_CLOSE,
            M_ATK_UNDEAD_ARCHER,
            M_ATK_UNDEAD_MAGE,
            M_ATK_GOLEM,
            M_ATK_SLIME,
            M_ATK_BOSS_HAND_DOWN,
            M_ATK_BOSS_LASER, 
            M_ATK_BOSS_ORB_PHASE1,
            M_ATK_BOSS_ORB_PHASE2,
            M_ATK_BOSS_BREATH,
            M_ATK_BOSS_SWING,
            M_ATK_BOSS_BREATH_CHARGE,
        }
        public enum Projectile
        {
            M_PROJ_UNDEAD_MAGE_CREATE,
            M_PROJ_UNDEAD_MAGE_SHOT,
        }
    }
    public static class Item
    {
        public enum Box
        {
            ITEM_BOX_SHAKE,
            ITEM_BOX_OPEN,
            ITEM_BOX_LEGENDARY_ITEM,
            ITEM_BOX_COLLECT_ITEM,
        }
        public enum Effect
        {
            ITEM_EFFECT_WIND,
            ITEM_EFFECT_THUNDER,
            ITEM_EFFECT_HOWLING,
        }
    }
    public enum Direction
    {
        DIR_BOSS_START,
        DIR_STAGE_CLEAR,
        DIR_DEATH_SCREEN_DARKEN,
        DIR_DEATH_SCREEN_LIGHTEN,
        DIR_DEATH_TEXT,
        DIR_PLAYER_SPAWN,
    }
    public enum BGM
    {
        BGM_BASE,
        BGM_BOSS_PHASE1,
        BGM_BOSS_PHASE2,
        BGM_Intro,
    }
    public enum UI
    {
        UI_CLICK,
        UI_TITLE
    }
    public enum Trait
    {
        TRAIT_NPC_INTERACT,
        TRAIT_SUCCESS,
        TRAIT_FAIL,
    }
    public enum Environment
    {
        ENV_PORTAL,
        ENV_GOLD_DESTROY,
        ENV_GOLD_COLLECT,
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
    [SerializeField]
    float Volume = 1.0f;
    public float GetVolume { get { return Volume; } }
}
