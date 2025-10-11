using UnityEngine;
using UnityEngine.Animations;

public class BackgroundSet : MonoBehaviour
{
    private GameObject _camera;

    private void Start()
    {
        _camera = MapManager.Instance.CameraObj;

        PositionConstraint constraint = GetComponent<PositionConstraint>();
        constraint.SetSources(new System.Collections.Generic.List<ConstraintSource>());

        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = _camera.transform,
            weight = 1f
        };

        constraint.AddSource(source);
        constraint.constraintActive = true;
    }
}
