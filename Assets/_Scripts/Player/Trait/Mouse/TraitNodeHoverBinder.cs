using UnityEngine;
using UnityEngine.UI;

namespace Game.Traits.UI
{
    [RequireComponent(typeof(Button))]
    public class TraitNodeHoverBinder : MonoBehaviour
    {
        [SerializeField] private TraitTooltipBuilder _builder;

        void Reset()
        {
            if (_builder == null) _builder = GetComponent<TraitTooltipBuilder>();
            if (GetComponent<TooltipHover>() == null) gameObject.AddComponent<TooltipHover>();
        }
    }
}
