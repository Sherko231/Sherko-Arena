using Sherko.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "UpDraftAbility", menuName = "Abilities/UpDraftAbility")]
public class UpDraftAbility : PlayerAbility
{
    [SerializeField] private float upDraftPower = 20f;
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
        if (player.Network.IsOwner)
        {
            Vector3 velo = player.Rb.velocity;
            player.Rb.velocity = new Vector3(velo.x, 0, velo.z);
            player.Rb.AddForce(Vector3.up * upDraftPower, ForceMode.Impulse);
        }
        Instantiate(particlePrefab, player.transform.position + Vector3.down, Quaternion.identity, player.transform);
    }
    
}
