using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GrenadeAbility", menuName = "Abilities/GrenadeAbility", order = 0)]
public class GrenadeAbility : PlayerAbility
{
    [Title("Settings")]
    [SerializeField] private float shootPower = 10f;
    [Title("References")] 
    [SerializeField] private Grenade grenadePrefab;
    
    protected override void ExecuteAbility(Player player)
    {
        Vector3 shootPoint = player.CamPoint.position;
        Vector3 shootDir = player.LookingVec;

        Grenade grenade = Instantiate(grenadePrefab, shootPoint, Quaternion.identity); //todo : to pool maybe ? 
        grenade.ThrowerClientId = player.Network.OwnerClientId;
        grenade.Launch(shootDir, shootPower);
    }
}