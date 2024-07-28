using UnityEngine;

public struct GunHit
{
    public GunHit(bool isValid, Vector3 hitPoint, Vector3 dir, GameObject hitObject)
    {
        IsValid = isValid;
        HitPoint = hitPoint;
        ShootDirection = dir;
        HitObject = hitObject;
    }
        
    public bool IsValid { get; private set; }
    public Vector3 HitPoint { get; private set; }
    public Vector3 ShootDirection { get; private set; }
    public GameObject HitObject { get; private set; }
}