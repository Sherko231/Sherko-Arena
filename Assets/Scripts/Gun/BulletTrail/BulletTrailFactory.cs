using UnityEngine;

public static class BulletTrailFactory
{
    public static TypedTrailRenderer Create(bool isLocal, Vector3 position, BulletTrailPoolType type)
    {
        TrailRenderer instance = BulletTrailPoolRegistry.I.GetPool(type).Pool.Get(); 
        if (isLocal) instance.gameObject.layer = LayerMask.NameToLayer("LocalBullet");
        instance.transform.position = position;
        instance.Clear();

        return new TypedTrailRenderer(type, instance);
    }
}
