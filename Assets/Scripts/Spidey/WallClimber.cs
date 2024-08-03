using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class WallClimber : MonoBehaviour
{
    [SerializeField, Required] private ClimbWallChecker climbWallChecker;
    [SerializeField, Required] private PlayerStats playerStats;
    [SerializeField] private float climbSpeed = 9f;
    private Player _player;

    private bool _IsWalled => climbWallChecker.IsWalled;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        if (!_IsWalled) return;

        Climb();
    }

    private void Climb()
    {
        if (!_player.Network.IsOwner) return;
        Rigidbody playerRB = _player.Rb;
        Vector3 moveVec = _player.Controller.MoveVec;
        Vector3 moveY = climbSpeed * moveVec.y * _player.transform.up;
        playerRB.velocity = new(playerRB.velocity.x, moveY.magnitude, playerRB.velocity.z);
    }
}
