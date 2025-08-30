using UnityEngine;

public class PlayerBob : MonoBehaviour
{
    public float walkingBobFrequency = 1.5f;
    public float walkingBobYRange = 1f;       // degrees, max yaw offset
    public float walkingBobXRange = 0.2f;        // vertical movement
    public float walkingBobRotationSpeed = 4f; // speed of rotation smoothing
    public float sidewaysTiltRange = 1.6f;
    public float straightTiltRange = 0.5f;
    public float tiltRotationSpeed = 5f;

    float timer = 0f;

    Vector3 startingPosition;
    Vector3 startingRotation;
    float bobYaw;

    void Start()
    {
        startingPosition = transform.localPosition;
        bobYaw = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    public void Neutral()
    {
        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.x = DoRotate(currentRotation.x, startingRotation.x, walkingBobRotationSpeed * Time.deltaTime);
        currentRotation.y = DoRotate(currentRotation.y, startingRotation.y, walkingBobRotationSpeed * Time.deltaTime);
        currentRotation.z = DoRotate(currentRotation.z, startingRotation.z, walkingBobRotationSpeed * Time.deltaTime);
        transform.localEulerAngles = currentRotation;

        transform.localPosition = Vector3.Lerp(transform.localPosition, startingPosition, walkingBobRotationSpeed * Time.deltaTime);
    }

    public void DoWalkingHeadBob(float velocity)
    {
        float radians = 2f * Mathf.PI * walkingBobFrequency * timer;

        float targetYaw = Mathf.Sin(radians) * walkingBobYRange;

        float rotationSpeedThisFrame = walkingBobRotationSpeed * Time.deltaTime * (2.3f * velocity);
        float newYaw = DoRotate(bobYaw, targetYaw, rotationSpeedThisFrame);
        bobYaw = newYaw;

        Vector3 currentEuler = transform.localEulerAngles;
        currentEuler.y = newYaw;
        transform.localEulerAngles = currentEuler;

        float targetY = startingPosition.y + (BellCurve(Mathf.Sin(radians)) * walkingBobXRange);
        Vector3 currentPosition = transform.localPosition;
        currentPosition.y = Mathf.Lerp(currentPosition.y, targetY, walkingBobRotationSpeed * 1.25f * Time.deltaTime * (2.5f * velocity));
        transform.localPosition = currentPosition;
    }

    public void DoSidewaysTilt(float direction)
    {
        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.z = DoRotate(currentRotation.z, direction * sidewaysTiltRange, tiltRotationSpeed * Time.deltaTime);
        currentRotation.x = DoRotate(currentRotation.x, startingRotation.x, tiltRotationSpeed * Time.deltaTime);

        transform.localEulerAngles = currentRotation;
    }

    public void DoStraightTilt(float direction)
    {
        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.x = DoRotate(currentRotation.x, direction * straightTiltRange, tiltRotationSpeed * Time.deltaTime);
        currentRotation.z = DoRotate(currentRotation.z, startingRotation.z, tiltRotationSpeed * Time.deltaTime);

        transform.localEulerAngles = currentRotation;
    }

    // Helper to wrap angle to [-180, 180] degrees
    private float WrapAngle180(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        else if (angle < -180f) angle += 360f;
        return angle;
    }

    private float GetRotationDelta(float origin, float target)
    {
        float rotationDelta = target - origin;
        rotationDelta = WrapAngle180(rotationDelta);
        return rotationDelta;
    }

    private float DoRotate(float originalRotation, float targetRotation, float speed)
    {
        float rotationDelta = GetRotationDelta(originalRotation, targetRotation);
        float step = speed;

        if (Mathf.Abs(rotationDelta) <= step)
            return targetRotation; // close enough

        if (rotationDelta > 0)
            return WrapAngle180(originalRotation + step);
        else
            return WrapAngle180(originalRotation - step);
    }

    private float BellCurve(float x)
    {
        return Mathf.Exp(-x * x) - Mathf.Exp(0f) + (1f - Mathf.Exp(-1f));
    }
}