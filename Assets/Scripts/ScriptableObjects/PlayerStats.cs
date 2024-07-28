using UnityEngine;

[CreateAssetMenu(fileName = "New Player Stats", menuName = "Player/Stats")]
public class PlayerStats : ScriptableObject
{
    public float MoveSpeed = 5f;
    public float SprintMultiplier = 3f; // will be multiplied by MoveSpeed
    [Range(0, 1)] public float SneakMultiplier = 0.5f;
    public float JumpForce = 5f;
    public float MaxSpeed => MoveSpeed * 3;
}
