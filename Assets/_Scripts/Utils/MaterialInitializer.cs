using UnityEngine;

public class MaterialInitializer : MonoBehaviour
{
    [SerializeField] private Material _glitchMaterial;
    [SerializeField] private Material _glitchMaterial2;
    [SerializeField] private Material _playerDefaultMaterial;

    private readonly string _glitchProperty = "_GlitchIntensity";
    private readonly string _glitchJump = "_VerticalJump";
    private readonly string _glitchShake = "_HorizontalShake";

    private void Start()
    {
        ResetGlitch();
    }

    public void addGlitch(float a)
    {
        if (_glitchMaterial == null) return;

        float crtValue = _glitchMaterial.GetFloat(_glitchProperty);
        float newValue = crtValue + a;

        _glitchMaterial.SetFloat(_glitchProperty, newValue);
    }

    public void removeGlitch(float a)
    {
        if (_glitchMaterial == null) return;

        float crtValue = _glitchMaterial.GetFloat(_glitchProperty);
        float newValue = crtValue - a;

        _glitchMaterial.SetFloat(_glitchProperty, newValue);
    }

    public void ResetGlitch()
    {
        if (_glitchMaterial != null)
        {
            _glitchMaterial.SetFloat(_glitchProperty, 0f);
            _glitchMaterial.SetFloat(_glitchJump, 0f);
            _glitchMaterial.SetFloat(_glitchShake, 0f);
        }
    }

    public void SetGlitchMaterial()
    {
        PlayerManager.Instance.Player.SpriteRenderer.material = _glitchMaterial2;
    }

    public void SetDefaultMaterial()
    {
        PlayerManager.Instance.Player.SpriteRenderer.material = _playerDefaultMaterial;
    }
}