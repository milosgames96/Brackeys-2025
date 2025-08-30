using UnityEngine;

public class ChamberController : MonoBehaviour
{
    public GameObject chamberCamera;
    public Transform chamberCameraTarget;
    public float cameraLerpSpeed = 1.3f;

    [HideInInspector]
    public bool isPlayerInside;

    private Animator animator;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInside)
        {
            chamberCamera.transform.localPosition = Vector3.Lerp(chamberCamera.transform.localPosition, chamberCameraTarget.localPosition, cameraLerpSpeed * Time.deltaTime);
        }
    }

    public void CloseChamber()
    {
        chamberCamera.SetActive(false);   
    }
}
