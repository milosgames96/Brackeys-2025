using UnityEngine;

public class PickUp : MonoBehaviour
{

    public GameObject pickUpObject;
    public Collectable collectable;
    public float rotationSpeed = 45f;
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 1f;

    private Vector3 startingPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition = pickUpObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        pickUpObject.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        // Float up and down
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        pickUpObject.transform.position = new Vector3(startingPosition.x, startingPosition.y + yOffset, startingPosition.z);
    }
}
