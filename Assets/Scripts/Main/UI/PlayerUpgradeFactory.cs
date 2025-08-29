using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;
using System.Linq;

public class PlayerUpgradeFactory : MonoBehaviour
{

    public GameObject upgradeFactoryScreenUI;
    public GameObject fillingsContainer;
    public GameObject fillingEntryPrefab;
    public GameObject upgradeEntryPrefab;
    public Slider fillingCapacitySlider;
    public GameObject fillingCapacityOverfilledText;
    public TextMeshProUGUI statsPreviewText;
    public Button doneButton;
    private List<FillingEntryController> fillingEntryControllers = new List<FillingEntryController>();
    private List<UpgradeEntryController> upgradeEntryControllers = new List<UpgradeEntryController>();
    private PlayerProfile playerProfile;
    private PlayerProfileModifierBuilder playerProfileModifierBuilder;
    private GameObject upgradeChamber;
    private Action<PlayerProfileModifier, List<Collectable>> DoneCallback;
    private PlayerInventory consumedCollectablesInventory;

    void Start()
    {
        fillingCapacitySlider.gameObject.SetActive(false);
        fillingCapacityOverfilledText.SetActive(false);
        doneButton.gameObject.SetActive(false);
        playerProfileModifierBuilder = new PlayerProfileModifierBuilder();
        consumedCollectablesInventory = new PlayerInventory();
    }

    public void Display(List<Collectable> fillings, List<Collectable> upgrades, PlayerProfile playerProfile, GameObject upgradeChamber, Action<PlayerProfileModifier, List<Collectable>> DoneCallback)
    {
        this.playerProfile = playerProfile;
        this.upgradeChamber = upgradeChamber;
        this.DoneCallback = DoneCallback;
        upgradeChamber.transform.Find("ChamberCamera").gameObject.SetActive(true);
        fillingCapacitySlider.gameObject.SetActive(true);
        fillingCapacitySlider.minValue = 0;
        fillingCapacitySlider.maxValue = playerProfile.fillingCapacity;
        fillingCapacitySlider.value = playerProfile.fillingAmount;
        doneButton.onClick.AddListener(DoneAndApply);
        doneButton.gameObject.SetActive(true);
        statsPreviewText.gameObject.SetActive(true);
        statsPreviewText.text = "";
        PopulateFillingEntries(fillings);
        PopulateUpgradeEntries(upgrades);
    }

    public void Hide()
    {
        Destroy(upgradeChamber);
        foreach (FillingEntryController fillingEntryController in fillingEntryControllers)
        {
            Destroy(fillingEntryController.gameObject);
        }
        foreach (UpgradeEntryController upgradeEntryController in upgradeEntryControllers)
        {
            Destroy(upgradeEntryController.gameObject);
        }
        fillingEntryControllers.Clear();
        upgradeEntryControllers.Clear();
        fillingCapacitySlider.gameObject.SetActive(false);
        fillingCapacityOverfilledText.SetActive(false);
        doneButton.gameObject.SetActive(false);
        statsPreviewText.gameObject.SetActive(false);
    }

    private void DoneAndApply()
    {
        PlayerProfileModifier playerProfileModifier = playerProfileModifierBuilder.Build();
        Hide();
        List<Collectable> consumedCollectables = new List<Collectable>();
        consumedCollectables.AddRange(consumedCollectablesInventory.GetFillings());
        consumedCollectables.AddRange(consumedCollectablesInventory.GetUpgrades());
        playerProfile.fillingAmount = fillingCapacitySlider.value;
        DoneCallback(playerProfileModifier, consumedCollectables);
    }

    private void PopulateFillingEntries(List<Collectable> fillings)
    {
        Vector3 currentEntryPosition = Vector3.zero;
        foreach (Collectable filling in fillings)
        {
            //init
            GameObject fillingEntry = Instantiate(fillingEntryPrefab, fillingsContainer.transform);
            fillingEntry.transform.position = currentEntryPosition;
            //position
            RectTransform rectTransform = fillingEntry.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = currentEntryPosition;
            currentEntryPosition.y -= rectTransform.rect.height;
            //controller
            FillingEntryController fillingEntryController = fillingEntry.GetComponent<FillingEntryController>();
            fillingEntryController.Init(filling, FillingEntrySliderChangedCallback);
            fillingEntryControllers.Add(fillingEntryController);
        }
    }

    private void PopulateUpgradeEntries(List<Collectable> upgrades)
    {
        Vector3 currentEntryPosition;
        if (fillingEntryControllers.Count > 0)
        {
            FillingEntryController lastFillingEntryController = fillingEntryControllers.Last();
            RectTransform rectTransform = lastFillingEntryController.GetComponent<RectTransform>();
            currentEntryPosition = rectTransform.anchoredPosition;
            currentEntryPosition.y -= rectTransform.rect.height;
        }
        else
        {
            currentEntryPosition = Vector3.zero;
        }
        foreach (Collectable upgrade in upgrades)
        {
            //init
            GameObject upgradeEntry = Instantiate(upgradeEntryPrefab, fillingsContainer.transform);
            upgradeEntry.transform.position = currentEntryPosition;
            //position
            RectTransform rectTransform = upgradeEntry.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = currentEntryPosition;
            currentEntryPosition.y -= rectTransform.rect.height;
            //controller
            UpgradeEntryController upgradeEntryController = upgradeEntry.GetComponent<UpgradeEntryController>();
            upgradeEntryController.Init(upgrade, UpgradeToggleChangedCallback);
            upgradeEntryControllers.Add(upgradeEntryController);
        }
    }

    private void UpdateFillingCapacitySlider()
    {
        float amount = playerProfile.fillingAmount;
        foreach (FillingEntryController fillingEntryController in fillingEntryControllers)
        {
            amount += fillingEntryController.GetAmount();
        }
        fillingCapacitySlider.value = amount;
        if (amount > playerProfile.fillingCapacity)
        {
            fillingCapacityOverfilledText.SetActive(true);
            doneButton.enabled = false;
        }
        else
        {
            fillingCapacityOverfilledText.SetActive(false);
            doneButton.enabled = true;
        }
    }

    private void UpdateStatsPreview()
    {
        StringBuilder statsStringBuilder = new StringBuilder();
        PlayerProfileModifier playerProfileModifier = playerProfileModifierBuilder.Peek();
        foreach (PlayerProfileModifier.ValueModifier valueModifier in playerProfileModifier.valueModifiers)
        {
            statsStringBuilder.Append(valueModifier.field);
            statsStringBuilder.Append(" : ");
            statsStringBuilder.Append(valueModifier.value);
            if (valueModifier.type == PlayerProfileModifier.ValueModifier.ValueModifierType.PERCENTAGE)
            {
                statsStringBuilder.Append(" %");
            } 
            statsStringBuilder.AppendLine();
        }
        statsPreviewText.text = statsStringBuilder.ToString();
    }

    private void FillingEntrySliderChangedCallback(float value, Collectable filling)
    {
        consumedCollectablesInventory.RemoveCollectable(filling);
        if (value > 0)
        {
            playerProfileModifierBuilder.AddFilling(filling, (int)value);
            consumedCollectablesInventory.InsertCollectable(filling.collectableType, (int)value);
        }
        else
        {
            playerProfileModifierBuilder.RemoveFilling(filling);
        }
        UpdateFillingCapacitySlider();
        UpdateStatsPreview();
    }

    private void UpgradeToggleChangedCallback(bool value, Collectable upgrade)
    {
        consumedCollectablesInventory.RemoveCollectable(upgrade);
        if (value)
        {
            playerProfileModifierBuilder.AddUpgrade(upgrade, 1);
            consumedCollectablesInventory.InsertCollectable(upgrade.collectableType, 1);
        }
        else
        {
            playerProfileModifierBuilder.RemoveUpgrade(upgrade);
        }
        UpdateStatsPreview();
    }
}
