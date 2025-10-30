using UnityEngine;
using UnityEngine.UI;

public class SliderBGM : MonoBehaviour
{
    private Slider _thisSlider;
    void Start()
    {
        _thisSlider = GetComponent<Slider>();
        Manager.Audio.BgmSlider = _thisSlider;
    }
}
