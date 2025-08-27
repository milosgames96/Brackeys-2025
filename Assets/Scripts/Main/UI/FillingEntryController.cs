using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using System;
using System.Runtime.CompilerServices;

public class FillingEntryController : MonoBehaviour
{

    public Slider slider;
    public TextMeshProUGUI maxText;
    public TextMeshProUGUI minText;
    private Collectable filling;
    private Action<float, Collectable> callback;

    public void Init(Collectable filling, Action<float, Collectable> callback)
    {
        this.filling = filling;
        this.callback = callback;
        maxText.text = filling.amount.ToString();
        minText.text = "0";
        slider.onValueChanged.AddListener(SliderValueChangedCallback);
        slider.minValue = 0;
        slider.maxValue = filling.amount;
    }

    public float GetAmount()
    {
        return slider.value;
    }

    private void SliderValueChangedCallback(float value)
    {
        minText.text = ((int)value).ToString();
        maxText.text = (filling.amount - (int)value).ToString();
        callback.Invoke(value, filling);
    }

}
