using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class PlayerUpgradeFactory : MonoBehaviour
{

    public GameObject upgradeFactoryScreenUI;
    public GameObject fillingsContainer;
    public GameObject fillingEntryPrefab;
    public Slider fillingCapacitySlider;
    public GameObject fillingCapacityOverfilledText;
    public TextMeshProUGUI statsPreviewText;
    private List<FillingEntryController> fillingEntryControllers = new List<FillingEntryController>();
    private PlayerProfile playerProfile;
    private PlayerProfileModifierBuilder fillingsPlayerProfileModifierBuilder;

    public void Init(PlayerProfile playerProfile)
    {
        this.playerProfile = playerProfile;
        this.fillingCapacitySlider.minValue = 0;
        this.fillingCapacitySlider.maxValue = playerProfile.fillingCapacity;
        this.fillingCapacitySlider.value = playerProfile.fillingAmount;
        this.fillingCapacityOverfilledText.SetActive(false);
        this.fillingsPlayerProfileModifierBuilder = new PlayerProfileModifierBuilder();
    }

    public void PopulateFillingSliders(List<Collectable> fillings)
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
