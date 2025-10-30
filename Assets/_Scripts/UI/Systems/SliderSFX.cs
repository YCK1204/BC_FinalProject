using UnityEngine;
using UnityEngine.UI;

public class SliderSFX : MonoBehaviour
{
    private Slider _thisSlider;
    void Start()
    {
        _thisSlider = GetComponent<Slider>();
        Manager.Audio.SfxSlider = _thisSlider;
    }
}
