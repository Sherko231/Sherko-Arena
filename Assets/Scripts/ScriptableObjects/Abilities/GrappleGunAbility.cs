using Sherko.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "GrappleGunAbility", menuName = "Abilities/GrappleGunAbility", order = 0)]
public class GrappleGunAbility : PlayerAbility
{
    public override bool CanBeUsed(PlayerStamina stamina)
    {
        Consumer consumer = stamina.Consumer;
        bool hasEnoughStamina = consumer.Current >= staminaCost;
        bool isUlting = stamina.IsUnlimited && consumer.Current < consumer.Capacity;
        return (hasEnoughStamina || isUlting) && CanUse;
    }

    protected override void ExecuteAbility(Player player)
    {
        GrappleGun grappleGun = player.CharacterSpecial.GetComponent<GrappleGun>();
        grappleGun.Shoot();
    }
}
