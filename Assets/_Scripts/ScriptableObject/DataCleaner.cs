using UnityEngine;

namespace _Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "DataCleaner", menuName = "ScriptableObject/DataCleaner")]
    public class DataCleaner : UnityEngine.ScriptableObject
    {
        public bool Intro;
        public bool Player;
        public bool Inventory;
    }
}