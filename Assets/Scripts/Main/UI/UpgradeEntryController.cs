using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeEntryController : MonoBehaviour
{

    public Toggle toggle;
    private Collectable upgrade;
    private Action<bool, Collectable> callback;

    public void Init(Collectable upgrade, Action<bool, Collectable> callback)
    {
        this.upgrade = upgrade;
        this.callback = callback;
        toggle.isOn = false;
        toggle.onValueChanged.AddListener(ToggleValueChangedCallback);
    }

    private void ToggleValueChangedCallback(bool value)
    {
        callback.Invoke(value, upgrade);
    }
}
