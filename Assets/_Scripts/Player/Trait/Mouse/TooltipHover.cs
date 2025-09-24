using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Traits.UI
{
    public class TooltipHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField] private TraitTooltipBuilder _builder;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_builder == null || TraitTooltip.Instance == null) return;
            TraitTooltip.Instance.Show(_builder.Build());
            TraitTooltip.Instance.LateReposition(eventData.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (TraitTooltip.Instance == null) return;
            TraitTooltip.Instance.Hide();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (TraitTooltip.Instance == null) return;
            TraitTooltip.Instance.LateReposition(eventData.position);
        }
    }
}
