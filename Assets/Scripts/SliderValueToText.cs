using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderValueToText : MonoBehaviour
{
    [SerializeField] Slider sliderUI;
    private Text textSliderValue;

    void Start()
    {
        textSliderValue = GetComponent<Text>();
        ShowSliderIntValue();
    }

    public void ShowSliderValue()
    {
        if (textSliderValue != null)
            textSliderValue.text = sliderUI.value.ToString("N2");

    }
    public void ShowSliderIntValue()
    {
        if (textSliderValue != null)

            textSliderValue.text = ((int)sliderUI.value).ToString();

    }
}