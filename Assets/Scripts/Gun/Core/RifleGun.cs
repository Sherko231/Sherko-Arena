using UnityEngine;

public class RifleGun : Gun
{
    private enum BulletType
    {
        HeavyRifle,
        LightRifle
    }
    
    [Header("Rifle")] 
    [SerializeField] private BulletType type;
    
    protected override TypedTrailRenderer CreateGunTrail()
    {
        BulletTrailPoolType targetPoolType = type == BulletType.LightRifle ? BulletTrailPoolType.LightRifle : BulletTrailPoolType.HeavyRifle;
        return BulletTrailFactory.Create(player.Network.IsOwner, shootPoint.position, targetPoolType);
    }

    protected override GunHit[] ShootRay()
    {
        bool isAimHit = Physics.Raycast(player.CamPoint.position, player.LookingVec, out RaycastHit hit, Mathf.Infinity, targetLayers);
        Vector3 hitPoint = isAimHit ? hit.point : player.CamPoint.position + (player.LookingVec * stats.MissBulletDist);
        Vector3 shootDir = (hitPoint - shootPoint.position).normalized;

        return new[]
        {
            new GunHit(isAimHit, hitPoint, shootDir, isAimHit ? hit.transform.gameObject : null)
        };
    }
}
