using System;
using System.Collections;
using Sherko.Utils;
using UnityEngine;

public abstract class PlayerAbility : ScriptableObject
{
    public float StaminaCost => staminaCost;

    [SerializeField] private int index;
    [SerializeField] protected float staminaCost = 25f;
    [SerializeField] protected float reuseDelay = 1.5f;
    
    public event Action OnExecution;
    public event Action OnOwnerExecution;
    public event Action OnReactivate;
    public event Action OnTermination;

    protected bool CanUse { get; private set; } = true;

    public void Execute(Player player)
    {
        ExecuteAbility(player);
        if (player.Network.IsOwner)
        {
            player.RpcManager.SendAbilityClientRpc(player.Network.OwnerClientId, index);
            player.StartCoroutine(StartReactivationTimer());
            OnOwnerExecution?.Invoke();
        }
        
        OnExecution?.Invoke();
    }

    protected abstract void ExecuteAbility(Player player);

    public virtual bool CanBeUsed(PlayerStamina stamina)
    {
        Consumer consumer = stamina.Consumer;
        bool hasEnoughStamina = consumer.Current >= staminaCost;
        return hasEnoughStamina && CanUse;
    }

    public virtual void TerminateAbility(Player player)
    {
        OnTermination?.Invoke();
    }

    private IEnumerator StartReactivationTimer()
    {
        CanUse = false;
        yield return new WaitForSeconds(reuseDelay);
        CanUse = true;
        OnReactivate?.Invoke();
    }
    
}
