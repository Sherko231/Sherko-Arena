using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletTrailPoolRegistry : MonoBehaviour
{
    public static BulletTrailPoolRegistry I { get; private set; }
    
    [SerializeField] private List<BulletTrailPool> pools;

    private void Awake()
    {
        I = this;
    }

    public BulletTrailPool GetPool(BulletTrailPoolType type)
    {
        BulletTrailPool target = null;
        foreach (BulletTrailPool trailPool in pools.Where(pool => pool.Type == type))
        {
            target = trailPool;
        }

        return target;
    }
}
