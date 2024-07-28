using UnityEngine;

public class TypedTrailRenderer
{
    public BulletTrailPoolType Type { get; private set; }
    public TrailRenderer Trail { get; private set; }
    
    public TypedTrailRenderer(BulletTrailPoolType type, TrailRenderer trail)
    {
        Type = type;
        Trail = trail;
    }
}
