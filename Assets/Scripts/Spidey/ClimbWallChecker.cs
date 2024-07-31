using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ClimbWallChecker : MonoBehaviour
{ 
    public bool IsWalled { get; private set; }
    [SerializeField, MinValue(0.1f)] private float radius = 1f;
    [SerializeField] private LayerMask targetLayers;

    private void FixedUpdate()
    {
        IsWalled = Physics.CheckSphere(transform.position, radius, targetLayers);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
