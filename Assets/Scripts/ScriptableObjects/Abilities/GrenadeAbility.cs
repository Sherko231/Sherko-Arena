using UnityEngine;

[CreateAssetMenu(fileName = "GrenadeAbility", menuName = "Abilities/GrenadeAbility", order = 0)]
public class GrenadeAbility : PlayerAbility
{
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private Grenade grenadePrefab;
    
    protected override void ExecuteAbility(Player player)
    {
        Vector3 shootPoint = player.CamPoint.position;
        Vector3 shootDir = player.LookingVec;

        Grenade grenade = Instantiate(grenadePrefab.gameObject, shootPoint, Quaternion.identity).GetComponent<Grenade>(); //todo : to pool maybe ? 
        grenade.ThrowerClientId = player.Network.OwnerClientId;
        grenade.Launch(shootDir, shootPower);
    }
}