using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [Header("DEV Tweaks")]
    public float mouseSensitivity = 0.1f;

    Transform orientationOfCamera;
    float mouseX_rotation; 
    float mouseY_rotation; 
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        if (Camera.main != null)
        {
            orientationOfCamera = Camera.main.transform;

        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }

    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            // Stop the input when game is paused
            return;
        }

        var xInput = Input.GetAxis("Mouse X");
        var yInput = Input.GetAxis("Mouse Y");

        // Not sure about Time.deltaTime here, will check later when we lock FPS ?
        mouseY_rotation += xInput * mouseSensitivity; // * Time.deltaTime; 
        mouseX_rotation -= yInput * mouseSensitivity; // * Time.deltaTime;
        mouseX_rotation = Mathf.Clamp(mouseX_rotation, -90f, 90f);

        if (orientationOfCamera != null)
        {
            orientationOfCamera.localRotation = Quaternion.Euler(mouseX_rotation, 0f, 0f);
        }
        transform.localRotation = Quaternion.Euler(0f, mouseY_rotation, 0f);
    }

    public void ResetCamera()
    {
        orientationOfCamera.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

}