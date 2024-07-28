namespace Sherko.Health
{
    public interface IDamager
    {
        float DamageAmount { get; }
        void Attack(IDamagable target);
    }
}

