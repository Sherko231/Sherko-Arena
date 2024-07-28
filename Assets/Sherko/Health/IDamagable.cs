using System;

namespace Sherko.Health
{
    public delegate void HealthChangeEventHandler(float currentHealth);

    public interface IDamagable
    {
        event Action OnDeath;
        event HealthChangeEventHandler OnHealthChange;

        float MaxHealth { get; }
        float CurrentHealth { get; }
        bool IsAlive { get; }

        void TakeDamage(float damage);
        void Heal(float heal);
        void Die();
        
        
    }
}

