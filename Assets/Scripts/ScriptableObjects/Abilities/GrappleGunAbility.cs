using UnityEngine;

[CreateAssetMenu(fileName = "GrappleGunAbility", menuName = "Abilities/GrappleGunAbility", order = 0)]
public class GrappleGunAbility : PlayerAbility
{
    protected override void ExecuteAbility(Player player)
    {
        GrappleGun grappleGun = player.CharacterSpecial.GetComponent<GrappleGun>();
        grappleGun.Shoot();
    }
}
