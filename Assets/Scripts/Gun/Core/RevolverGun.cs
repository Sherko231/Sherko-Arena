using UnityEngine;

public class RevolverGun : Gun
{
    protected override TypedTrailRenderer CreateGunTrail()
    {
        return BulletTrailFactory.Create(player.Network.IsOwner, shootPoint.position, BulletTrailPoolType.Revolver);
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
