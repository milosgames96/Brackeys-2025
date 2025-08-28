using UnityEngine;

public class PickUp : MonoBehaviour
{

    public GameObject pickUpPrefab;
    public Collectable collectable;
    private GameObject pickUpObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pickUpObject = Instantiate(pickUpPrefab, transform);
        pickUpObject.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
