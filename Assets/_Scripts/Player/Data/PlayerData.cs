using UnityEngine;

namespace GameSystem
{
    [System.Serializable]
    public class PlayerGroundData
    {
        public float BaseSpeed = 6f;
        public float WalkSpeedModifier = 1f;
    }

    [System.Serializable]
    public class PlayerAirData
    {
        public float JumpForce = 3f;
    }

    [System.Serializable]
    public class PlayerData
    {
        public PlayerGroundData GroundData = new PlayerGroundData();
        public PlayerAirData AirData = new PlayerAirData();
    }
}
