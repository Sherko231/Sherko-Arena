using Sherko.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "DashAbility", menuName = "Abilities/DashAbility")]
public class DashAbility : PlayerAbility
{
    [SerializeField] private float dashPower = 100f;
    [SerializeField] private GameObject particlePrefab;
    
    public override bool CanBeUsed(PlayerStamina stamina)
    {
        Consumer consumer = stamina.Consumer;
        bool hasEnoughStamina = consumer.Current >= staminaCost;
        bool isUlting = stamina.IsUnlimited && consumer.Current < consumer.Capacity;
        return (hasEnoughStamina || isUlting) && CanUse;
    }

    protected override void ExecuteAbility(Player player)
    {
        Instantiate(particlePrefab, player.transform.position, Quaternion.identity, player.transform);
        if (player.Network.IsOwner)
        {
            player.CanMove = false;
            player.Rb.AddForce(player.MovingVec * dashPower, ForceMode.Impulse);
        }
    }

    public override void TerminateAbility(Player player)
    {
        player.CanMove = true;
        base.TerminateAbility(player);
    }
}
