using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;

public class FillingEntryController : MonoBehaviour
{
    [Serializable]
    public class FillingSpriteLookupPair
    {
        public Collectable.CollectableType collectableType;
        public Sprite sprite; 
    }

    public Slider slider;
    public Image image;
    public TextMeshProUGUI maxText;
    public TextMeshProUGUI minText;
    public List<FillingSpriteLookupPair> spriteLookup;
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
        FillingSpriteLookupPair spriteLookupPair = spriteLookup
            .Where(pair => pair.collectableType == filling.collectableType)
            .First();
        if (spriteLookupPair != null)
        {
            image.sprite = spriteLookupPair.sprite;    
        }
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
