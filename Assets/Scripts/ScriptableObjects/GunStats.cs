using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Stats", menuName = "Gun/GunStats")]
public class GunStats : ScriptableObject
{
    public float Damage = 8;
    public float ShootInterval = 0.3f;
    public float AmmoCapacity = 120;
    public float MissBulletDist = 120;
    [Tooltip("In Seconds")] public float ReloadTime = 0.5f;
}
