using UnityEngine;
using UnityEngine.Pool;

public class BulletTrailPool : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TrailRenderer trailPrefab;
    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;
    [SerializeField] private BulletTrailPoolType type;
    
    public ObjectPool<TrailRenderer> Pool { get; private set; }
    public BulletTrailPoolType Type => type;

    private void Awake()
    {
        Pool = new ObjectPool<TrailRenderer>(CreateBullet, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, false, defaultCapacity, maxSize);
    }

    private TrailRenderer CreateBullet()
    {
        TrailRenderer instance = Instantiate(trailPrefab, transform);
        return instance;
    }

    private void OnGetFromPool(TrailRenderer bullet)
    {
        bullet.gameObject.SetActive(true);
        bullet.gameObject.layer = LayerMask.NameToLayer("Bullet");
    }

    private void OnReleaseToPool(TrailRenderer bullet)
    {
        bullet.gameObject.layer = LayerMask.NameToLayer("Bullet");
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyPooledObject(TrailRenderer bullet)
    {
        Destroy(bullet.gameObject);
    }
}

public enum BulletTrailPoolType
{
    LightRifle,
    HeavyRifle,
    Shotgun
}