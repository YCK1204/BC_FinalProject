using UnityEngine;

namespace Game.Traits.UI
{
    public class DescriptionDB : MonoBehaviour
    {
        public static DescriptionDB Instance { get; private set; }

        [TextArea][SerializeField] private string _desc9203 = "느리게 움직이는 초음파를 내보내 적에게 큰 피해를 입힙니다.";
        [TextArea][SerializeField] private string _desc9204 = "각성 상태가 되어 10초 동안 공격 속도와 스킬 가속이 증가합니다.";

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public string Get(int id)
        {
            switch (id)
            {
                case 9203: return _desc9203;
                case 9204: return _desc9204;
                default: return string.Empty;
            }
        }
    }
}
