using System;
using System.Collections;
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

    protected void Awake()
    {
        _animator = GetComponent<Animator>();
        Ammo = new Consumer(stats.AmmoCapacity);
        Ammo.Full();
    }

    public virtual void Init(Player p)
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
        foreach (GunHit gunHit in gunHits)
        {
            if (debug) Debug.DrawRay(shootPoint.position, gunHit.ShootDirection * 100, Color.red, 1);
            
            if (player.Network.IsOwner)
            {
                DetectAndDamagePlayer(gunHit);
            }
            
            VisualizeShoot(gunHit);
        }

        if (player.Network.IsOwner)
        {
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
    
    private void DetectAndDamagePlayer(GunHit gunHit) 
    {
        int hitLayer = gunHit.IsValid ? gunHit.HitObject.layer : 0;
        if (hitLayer != LayerMask.NameToLayer("Player") && hitLayer != LayerMask.NameToLayer("LocalPlayer")) return;
        
        Player target = OnlinePlayersRegistry.GetByInstanceId(gunHit.HitObject.GetInstanceID());
        if (!target.Health.IsAlive || target == player) return;
        
        target.Health.TakeDamage(stats.Damage);
        if (target.Health.CurrentHealth <= 0) player.IncreaseKill();
        player.RpcManager.SendHealthDamageClientRpc(target.Network.OwnerClientId, stats.Damage); //might be network heavy in for loop, keep in mind : (maybe more than one target)
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
