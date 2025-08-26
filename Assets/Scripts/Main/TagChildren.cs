using UnityEngine;

public class TagChildren : MonoBehaviour
{
    public string targetTag = "Untagged";

    void Start()
    {
        SetTagRecursively(transform, targetTag);
    }

    void SetTagRecursively(Transform parentTransform, string tagToSet)
    {
        parentTransform.gameObject.tag = tagToSet;

        foreach (Transform child in parentTransform)
        {
            SetTagRecursively(child, tagToSet);
        }
    }
}