using System;
using System.Collections.Generic;
using UnityEngine;

public class SideWallChecker : MonoBehaviour
{
    [SerializeField] private float radius = 0.7f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private Direction direction;
    
    [SerializeField] [Range(0f, 1f)]
    private float faceDetectionPrecision = 0.55f;
    
    [SerializeField] [Range(0f, 1f)] [Tooltip("Less = more precise")] 
    private float parallelViewThreshold = 0.35f;

    [SerializeField] [Range(25f, 90f)] 
    private float checkingThreshold = 60f;
    
    private Player _player;

    private enum Direction
    {
        Right,
        Left
    }
    
    public bool IsWalled { get; private set; }

    public void Init(Player p)
    {
        _player = p;
    }
    
    private void Update()
    {
        Collider[] hitColliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, targetLayers);
        Vector3 camForward = _player.CamPoint.forward;
        Vector3 wallCheckDir = direction == Direction.Right ? transform.right : -transform.right;
        float camEulerX = _player.CamPoint.transform.eulerAngles.x;
        if (camEulerX > 90) camEulerX -= 360;

        Collider col = hitColliders[0];
        if (!col)
        {
            IsWalled = false;
            return;
        }
        
        Transform objTrans = col.transform;
        IEnumerable<Vector3> objDirections = GetFaceDirections(objTrans);

        foreach (Vector3 objDir in objDirections)
        {
            bool isLookingDownOrUp = camEulerX < -checkingThreshold || camEulerX > checkingThreshold;
            bool isOppositeFace = Vector3.Dot(wallCheckDir, objDir) < -faceDetectionPrecision;
            bool isMovingIntoOppositeFace = Vector3.Dot(_player.MovingVec, objDir) < -faceDetectionPrecision;
            bool isLookingParallelToWall = Vector3.Dot(camForward, objDir) < parallelViewThreshold && Vector3.Dot(camForward, objDir) > -parallelViewThreshold;
            bool isGrounded = _player.IsGrounded;
                
            if (isOppositeFace && isMovingIntoOppositeFace && isLookingParallelToWall && !isGrounded && !isLookingDownOrUp)
            {
                IsWalled = true;
                return;
            }
        }
        
        
        IsWalled = false;
    }

    private static IEnumerable<Vector3> GetFaceDirections(Transform objTrans)
    {
        Vector3 right = objTrans.right;
        Vector3 forward = objTrans.forward;
        Vector3[] objDirections = { right, -right, forward, -forward };
        return objDirections;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
