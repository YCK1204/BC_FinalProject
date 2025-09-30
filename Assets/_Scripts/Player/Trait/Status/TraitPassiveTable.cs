using Game.Traits.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Traits
{
    [CreateAssetMenu(fileName = "TraitPassiveTable", menuName = "Game/Trait Passive Table")]
    public class TraitPassiveTable : ScriptableObject
    {
        [Serializable]
        public class Row
        {
            public int TraitId;
            public Ability Ability;
            public float Value;
            public bool IsPercent;
        }

        [SerializeField] List<Row> _rows = new List<Row>();
        Dictionary<int, List<Row>> _map;

        void OnEnable()
        {
            BuildMap();
        }

        public void BuildMap()
        {
            _map = new Dictionary<int, List<Row>>();
            foreach (var r in _rows)
            {
                if (!_map.TryGetValue(r.TraitId, out var list))
                {
                    list = new List<Row>();
                    _map.Add(r.TraitId, list);
                }
                list.Add(r);
            }
        }

        public IEnumerable<Row> GetRowsFor(int traitId)
        {
            if (_map == null) BuildMap();
            return _map != null && _map.TryGetValue(traitId, out var list) ? list : Array.Empty<Row>();
        }
    }
}
