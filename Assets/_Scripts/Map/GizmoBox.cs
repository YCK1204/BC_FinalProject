using UnityEngine;

public class GizmoBox : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(1, 1, 1);
    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}