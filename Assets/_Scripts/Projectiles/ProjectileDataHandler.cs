using UnityEngine;

public class ProjectileDataHandler : MonoBehaviour
{
    [SerializeField] private ProjectileData _data;
    public ProjectileData Data { get { return _data; } set { _data = value; } }

    public float Damage { get; set; }

    // 추후에 데이터 관련해서 접근해서 변경할 수 있으니 ProjectileDataHandler 부분은 유지
}
