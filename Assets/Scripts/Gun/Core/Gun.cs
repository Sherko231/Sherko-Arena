using System;
using System.Collections;
using System.Collections.Generic;
using Sherko.Utils;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    private static readonly int ShootAnimPrp = Animator.StringToHash("Shoot");
    private static readonly int ReloadAnimPrp = Animator.StringToHash("Reload");
    
    [Header("References")]
    [SerializeField] protected Transform shootPoint;
    [SerializeField] private ParticleSystem muzzleFlashVFX;
    [SerializeField] private ParticleSystem hitVFX;
    [Header("Properties")]
    [SerializeField] protected bool debug;
    [SerializeField] protected GunStats stats;
    [SerializeField] protected float trailSpeed = 500f;
    [Header("Shoot Ray Properties")]
    [SerializeField] protected LayerMask targetLayers;
    protected Player player;

    public Consumer Ammo { get; private set; }
    public GunStats Stats => stats;
    public Transform ShootPoint => shootPoint;
    private ulong playerClientId => player.Network.OwnerClientId;
    
    private bool _canShoot = true;
    private Animator _animator;

    public event Action OnShoot;
    public event Action OnShootOwner;
    public event Action OnInit; 

    protected abstract TypedTrailRenderer CreateGunTrail();
    protected abstract GunHit[] ShootRay();
    
    private class GunHitInfo
    {
        public GunHitInfo(ulong targetClientID, float damage, Vector3 shootDir)
        {
            TargetClientID = targetClientID;
            Damage = damage;
            ShootDir = shootDir;
        }
        
        public ulong TargetClientID { get; private set; }
        public float Damage { get; private set; }
        public Vector3 ShootDir { get; private set; }
    }
    
    private struct DamagePair
    {
        public DamagePair(float damage, Vector3 bulletVec)
        {
            Damage = damage;
            BulletVec = bulletVec;
        }
        
        public float Damage { get; private set; }
        public Vector3 BulletVec { get; private set; }
    }
    
    protected void Awake()
    {
        _animator = GetComponent<Animator>();
        Ammo = new Consumer(stats.AmmoCapacity);
        Ammo.Full();
    }

    public void Init(Player p)
    {
        player = p;
        if (player.Network.IsOwner) ApplyLocalLayer();
        OnInit?.Invoke();
    }

    public void Shoot()
    {
        if (Ammo.IsEmpty) return;
        if (!_canShoot) return;

        GunHit[] gunHits = ShootRay();
        List<GunHitInfo> gunHitInfos = new();
        foreach (GunHit gunHit in gunHits)
        {
            if (debug) Debug.DrawRay(shootPoint.position, gunHit.ShootDirection * 100, Color.red, 1);

            if (player.Network.IsOwner)
            {
                GunHitInfo info = DetectPlayersToDamage(gunHit);
                if (info != null) gunHitInfos.Add(info);
            }
            
            VisualizeShoot(gunHit);
        }

        if (player.Network.IsOwner)
        {
            Dictionary<ulong, DamagePair> damagePairs = CalculateDamages(gunHitInfos);
            player.StartCoroutine(DamagePlayers(damagePairs));
            
            player.RpcManager.SendPlayerShootClientRpc(playerClientId);
            OnShootOwner?.Invoke();
        }
        else
        {
            OnShoot?.Invoke();
        }
        Ammo.Consume(1);
    }

    public void Reload()
    {
        if (Ammo.IsFull) return;
        StartCoroutine(RefillAmmo());
        VisualizeReload();
        if (player.Network.IsOwner)
        {
            player.RpcManager.SendPlayerReloadClientRpc(playerClientId);
        }
    }
    
    private GunHitInfo DetectPlayersToDamage(GunHit gunHit) 
    {
        int hitLayer = gunHit.IsValid ? gunHit.HitObject.layer : 0;
        if (hitLayer != LayerMask.NameToLayer("Player") && hitLayer != LayerMask.NameToLayer("LocalPlayer")) return null;
        
        Player target = OnlinePlayersRegistry.GetByInstanceId(gunHit.HitObject.GetInstanceID());
        if (!target.Health.IsAlive || target == player) return null;

        return new GunHitInfo(target.Network.OwnerClientId, stats.Damage, shootPoint.forward);
    }

    private Dictionary<ulong, DamagePair> CalculateDamages(List<GunHitInfo> gunHitInfos) //clientId, (damage, bulletVec)
    {
        Dictionary<ulong, DamagePair> damagePairs = new();

        foreach (GunHitInfo gunHitInfo in gunHitInfos)
        {
            if (damagePairs.TryGetValue(gunHitInfo.TargetClientID, out DamagePair previousDamagePair))
            {
                damagePairs[gunHitInfo.TargetClientID] = new DamagePair(gunHitInfo.Damage + previousDamagePair.Damage, gunHitInfo.ShootDir);
            }
            else
            {
                damagePairs.Add(gunHitInfo.TargetClientID, new DamagePair(gunHitInfo.Damage, gunHitInfo.ShootDir));
            }
            
        }

        return damagePairs;
    }

    private IEnumerator DamagePlayers(Dictionary<ulong, DamagePair> damagePairs)
    {
        foreach (KeyValuePair<ulong, DamagePair> pair in damagePairs)
        {
            Player target = OnlinePlayersRegistry.Get(pair.Key);
            
            target.Health.TakeDamage(pair.Value.Damage, pair.Value.BulletVec);
            if (target.Health.CurrentHealth <= 0) player.IncreaseKill();
            player.RpcManager.SendHealthDamageClientRpc(target.Network.OwnerClientId, pair.Value.Damage, pair.Value.BulletVec);
            yield return new WaitForEndOfFrame();
        }
    }
    
    private void VisualizeShoot(GunHit gunHit)
    {
        StartCoroutine(ShootTrail(gunHit));
        _animator.SetTrigger(ShootAnimPrp);
        PlayHitVFX(gunHit.HitPoint);
        muzzleFlashVFX.Play();
    }

    private void VisualizeReload()
    {
        _animator.SetTrigger(ReloadAnimPrp);
    }

    private IEnumerator RefillAmmo()
    {
        _canShoot = false;
        yield return new WaitForSeconds(stats.ReloadTime);
        Ammo.Full();
        _canShoot = true;
    }
    
    private IEnumerator ShootTrail(GunHit gunHit)
    {
        Vector3 shootPointPos = shootPoint.position;
        TypedTrailRenderer rendererInstance = CreateGunTrail();
        float dist = Vector3.Distance(shootPointPos, gunHit.HitPoint);
        float remainingDist = dist;
        while (remainingDist > 0)
        {
            rendererInstance.Trail.transform.position = Vector3.Lerp(shootPointPos, gunHit.HitPoint, Mathf.Clamp01(1 - (remainingDist / dist)));
            remainingDist -= trailSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        BulletTrailPoolRegistry.I.GetPool(rendererInstance.Type).Pool.Release(rendererInstance.Trail);
    }

    private void PlayHitVFX(Vector3 position)
    {
        Instantiate(hitVFX.gameObject, position, Quaternion.identity); //hit vfx
    }
    
    private void ApplyLocalLayer()
    {
        foreach (Transform trans in GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = LayerMask.NameToLayer("LocalGun");
        }
        
        muzzleFlashVFX.gameObject.layer = LayerMask.NameToLayer("LocalBullet");
    }
}
