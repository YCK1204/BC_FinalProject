using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Traits.UI
{
    /// <summary>
    /// 같은 오브젝트에 붙은 TraitTooltipBuilder를 자동으로 찾아
    /// 마우스 호버 시 TraitTooltip을 보여준다.
    /// </summary>
    public class TooltipHoverAuto : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        TraitTooltipBuilder _builder;

        void Awake()
        {
            _builder = GetComponent<TraitTooltipBuilder>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_builder == null || TraitTooltip.Instance == null) return;
            TraitTooltip.Instance.Show(_builder.Build());
            TraitTooltip.Instance.LateReposition(eventData != null ? eventData.position : (Vector2)Input.mousePosition);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (TraitTooltip.Instance == null) return;
            TraitTooltip.Instance.Hide();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (TraitTooltip.Instance == null) return;
            TraitTooltip.Instance.LateReposition(eventData != null ? eventData.position : (Vector2)Input.mousePosition);
        }
    }
}
