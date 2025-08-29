using UnityEngine;

public class PickUpNotification : MonoBehaviour
{
    public float fallingSpeed;
    public float shrinkingFactor;
    public float duration;
    private RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        rectTransform = GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - fallingSpeed * Time.deltaTime);
            fallingSpeed += fallingSpeed * (1.23f * Time.deltaTime);
            rectTransform.sizeDelta *= 1f - (shrinkingFactor * Time.deltaTime);
        }
        else
        {
            rectTransform.sizeDelta *= 1f + (shrinkingFactor * Time.deltaTime);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + fallingSpeed * Time.deltaTime);
        }
    }
}
