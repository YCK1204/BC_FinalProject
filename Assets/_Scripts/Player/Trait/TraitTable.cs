using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Traits
{

    [Serializable]
    public struct TraitRow
    {
        public int Id;
        public int ConditionId;
        public TraitKind Kind;
        public int SoulCost;
    }

    [CreateAssetMenu(fileName = "TraitTable", menuName = "Game/Trait Table")]
    public class TraitTable : ScriptableObject
    {
        public List<TraitRow> Rows = new List<TraitRow>();

        public bool TryGet(int id, out TraitRow row)
        {
            for (int i = 0; i < Rows.Count; i++)
                if (Rows[i].Id == id) { row = Rows[i]; return true; }
            row = default;
            return false;
        }
    }
}
