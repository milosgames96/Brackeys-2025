using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "Scriptable Objects/PlayerProfile")]
public class PlayerProfile : ScriptableObject
{
    public float health;
    public float moveSpeed;
    public float jumpForce;
    public float acceleration;
}
