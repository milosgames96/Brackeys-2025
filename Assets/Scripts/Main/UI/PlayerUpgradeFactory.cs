using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;

public class PlayerUpgradeFactory : MonoBehaviour
{

    public GameObject upgradeFactoryScreenUI;
    public GameObject fillingsContainer;
    public GameObject fillingEntryPrefab;
    public Slider fillingCapacitySlider;
    public GameObject fillingCapacityOverfilledText;
    public TextMeshProUGUI statsPreviewText;
    public Button doneButton;
    private List<FillingEntryController> fillingEntryControllers = new List<FillingEntryController>();
    private PlayerProfile playerProfile;
    private PlayerProfileModifierBuilder fillingsPlayerProfileModifierBuilder;
    private GameObject chamberCamera;
    private Action<PlayerProfileModifier> DoneCallback;

    void Start()
    {
        fillingCapacitySlider.gameObject.SetActive(false);
        fillingCapacityOverfilledText.SetActive(false);
        doneButton.gameObject.SetActive(false);
        fillingsPlayerProfileModifierBuilder = new PlayerProfileModifierBuilder();
    }

    public void Display(List<Collectable> fillings, PlayerProfile playerProfile, GameObject chamberCamera, Action<PlayerProfileModifier> DoneCallback)
    {
        this.playerProfile = playerProfile;
        this.chamberCamera = chamberCamera;
        this.DoneCallback = DoneCallback;
        chamberCamera.SetActive(true);
        fillingCapacitySlider.gameObject.SetActive(true);
        fillingCapacitySlider.minValue = 0;
        fillingCapacitySlider.maxValue = playerProfile.fillingCapacity;
        fillingCapacitySlider.value = playerProfile.fillingAmount;
        doneButton.onClick.AddListener(DoneAndApply);
        doneButton.gameObject.SetActive(true);
        PopulateFillingSliders(fillings);
    }

    public void Hide()
    {
        chamberCamera.SetActive(false);
        foreach (FillingEntryController fillingEntryController in fillingEntryControllers)
        {
            Destroy(fillingEntryController.gameObject);
        }
        fillingEntryControllers.Clear();
    }

    private void DoneAndApply()
    {
        PlayerProfileModifier playerProfileModifier = fillingsPlayerProfileModifierBuilder.Build();
        Hide();
        DoneCallback(playerProfileModifier);
    }

    private void PopulateFillingSliders(List<Collectable> fillings)
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

    private void UpdateFillingCapacitySlider()
    {
        float amount = 0;
        foreach (FillingEntryController fillingEntryController in fillingEntryControllers)
        {
            amount += fillingEntryController.GetAmount();
        }
        fillingCapacitySlider.value = amount;
        this.fillingCapacityOverfilledText.SetActive(amount > playerProfile.fillingCapacity);
    }

    private void UpdateFillingStatsPreview()
    {
        StringBuilder statsStringBuilder = new StringBuilder();
        PlayerProfileModifier playerProfileModifier = fillingsPlayerProfileModifierBuilder.Peek();
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
        if (value == 0)
        {
            fillingsPlayerProfileModifierBuilder.RemoveFilling(filling);
        }
        else
        {
            fillingsPlayerProfileModifierBuilder.AddFilling(filling, (int)value);            
        }
        UpdateFillingCapacitySlider();
        UpdateFillingStatsPreview();
    }
}
