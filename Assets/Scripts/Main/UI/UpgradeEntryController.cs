using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeEntryController : MonoBehaviour
{
    [Serializable]
    public class UpgradeSpriteLookupPair
    {
        public Collectable.CollectableType collectableType;
        public Sprite sprite; 
    }

    public Toggle toggle;
    public Image image;
    public List<UpgradeSpriteLookupPair> spriteLookup;
    private Collectable upgrade;
    private Action<bool, Collectable> callback;

    public void Init(Collectable upgrade, Action<bool, Collectable> callback)
    {
        this.upgrade = upgrade;
        this.callback = callback;
        toggle.isOn = false;
        toggle.onValueChanged.AddListener(ToggleValueChangedCallback);
        UpgradeSpriteLookupPair spriteLookupPair = spriteLookup
            .Where(pair => pair.collectableType == upgrade.collectableType)
            .First();
        if (spriteLookupPair != null)
        {
            image.sprite = spriteLookupPair.sprite;    
        }
    }

    private void ToggleValueChangedCallback(bool value)
    {
        callback.Invoke(value, upgrade);
    }
}
