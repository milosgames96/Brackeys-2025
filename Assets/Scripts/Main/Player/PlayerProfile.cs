using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "Scriptable Objects/PlayerProfile")]
public class PlayerProfile : ScriptableObject
{
    public float health;
    public float maxSpeed;
    public float jumpForce;
    public float movementForce;
    public float brakingForce;
    public float fillingCapacity;
    public float fillingAmount;
}
