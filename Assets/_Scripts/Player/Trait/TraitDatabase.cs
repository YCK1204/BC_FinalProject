using System.Collections.Generic;
using UnityEngine;

namespace Game.Traits
{
    [CreateAssetMenu(fileName = "TraitDatabase", menuName = "Game/Trait Database")]
    public class TraitDatabase : ScriptableObject
    {
        public List<TraitData> Traits = new List<TraitData>();

        public TraitData GetById(int id)
        {
            for (int i = 0; i < Traits.Count; i++)
            {
                if (Traits[i].Id == id) return Traits[i];
            }
            return null;
        }
    }
}
