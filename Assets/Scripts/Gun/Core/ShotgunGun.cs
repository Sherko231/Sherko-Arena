using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotgunGun : Gun
{
    [Header("Shotgun")]
    [SerializeField] private int pelletCount = 10;
    [SerializeField] private float spreadOffset = 0.3f;

    protected override TypedTrailRenderer CreateGunTrail()
    {
        return BulletTrailFactory.Create(player.Network.IsOwner, shootPoint.position, BulletTrailPoolType.Shotgun);
    }

    protected override GunHit[] ShootRay()
    {
        List<GunHit> hits = new();
        for (int i = 0; i < pelletCount; i++) hits.Add(ShootPellet());
        return hits.ToArray();
    }

    private GunHit ShootPellet()
    {
        Vector3 randomOffsetEuler = CreateRandomOffsetEuler();
        Vector3 aimDir = player.LookingVec + randomOffsetEuler;
        bool isAimHit = Physics.Raycast(player.CamPoint.position, aimDir, out RaycastHit hit, Mathf.Infinity, targetLayers);
        Vector3 hitPoint = isAimHit ? hit.point : player.CamPoint.position + (aimDir * stats.MissBulletDist);
        Vector3 shootDir = (hitPoint - shootPoint.position).normalized;

        return new GunHit(isAimHit, hitPoint, shootDir, isAimHit ? hit.transform.gameObject : null);
    }

    private Vector3 CreateRandomOffsetEuler()
    {
        float randomX = Random.Range(-spreadOffset, spreadOffset);
        float randomY = Random.Range(-spreadOffset, spreadOffset);
        float randomZ = Random.Range(-spreadOffset, spreadOffset);
        return new Vector3(randomX, randomY, randomZ);
    }
    
}
