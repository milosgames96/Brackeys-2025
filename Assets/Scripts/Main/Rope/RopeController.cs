using System;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public GameObject ropeCamera;
    public List<Transform> ropeCameraPoints;
    public float ropeCameraSpeed = 17f;
    public float ropeCameraLookAtSpeed = 180f;
    public Transform exitPoint;
    private bool isPlayerInside;
    private Transform currentRopeCameraPoint;
    private GameObject player;
    private Action ExitCallback;

    void Start()
    {
        ropeCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInside)
        {
            UnrollRope();
        }
    }

    public void EnterRope(GameObject player, Action ExitCallback)
    {
        isPlayerInside = true;
        this.player = player;
        this.ExitCallback = ExitCallback; 
        ropeCamera.SetActive(true);
    }

    private void UnrollRope()
    {
        if (currentRopeCameraPoint == null)
        {
            if (ropeCameraPoints.Count > 0)
            {
                currentRopeCameraPoint = ropeCameraPoints[0];
                ropeCameraPoints.RemoveAt(0);
            }
            else
            {
                ExitRope();
                return;
            }
        }
        ropeCamera.transform.localPosition = Vector3.MoveTowards(
            ropeCamera.transform.localPosition,
            currentRopeCameraPoint.localPosition,
            ropeCameraSpeed * Time.deltaTime
        );
        Vector3 directionToLook = currentRopeCameraPoint.position - ropeCamera.transform.position;
        if (directionToLook != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            ropeCamera.transform.rotation = Quaternion.RotateTowards(
                ropeCamera.transform.rotation,
                targetRotation,
                ropeCameraLookAtSpeed * Time.deltaTime 
            );
        }
        if (Vector3.Distance(ropeCamera.transform.localPosition, currentRopeCameraPoint.localPosition) < 2.3f)
        {
            currentRopeCameraPoint = null;
        }
    }

    private void ExitRope()
    {
        isPlayerInside = false;
        ropeCamera.SetActive(false);
        player.transform.position = exitPoint.transform.position;
        ExitCallback();
    }
}