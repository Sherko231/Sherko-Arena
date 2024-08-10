using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [TabGroup("Radius")] [SerializeField] private float highDmgRadius = 2f;
    [TabGroup("Radius")] [SerializeField] private float mediumDmgRadius = 3.5f;
    [TabGroup("Radius")] [SerializeField] private float lowDmgRadius = 5f;
    [TabGroup("Damage")] [SerializeField] private float highDmg = 50f;
    [TabGroup("Damage")] [SerializeField] private float mediumDmg = 25f;
    [TabGroup("Damage")] [SerializeField] private float lowDmg = 10f;

    [Title("References")]
    [SerializeField] private ParticleSystem explodeVFX;
    
    public ulong ThrowerClientId { get; set; }

    private bool _IsLocalGrenade => OnlinePlayersRegistry.Get(ThrowerClientId).Network.IsOwner;
    
    private Rigidbody _rb;
    
    private struct PlayerDamageHandler
    {
        public Player player;
        public float damage;
    }
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, highDmgRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, mediumDmgRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, lowDmgRadius);
    }

    public void Launch(Vector3 dir, float shootPower)
    {
        _rb.AddForce(dir * shootPower, ForceMode.Impulse);
    }

    private void Explode()
    {
        Instantiate(explodeVFX.gameObject, transform.position, Quaternion.identity);
        if (_IsLocalGrenade)
        {
            PlayerDamageHandler[] handlers = DetectAffectedPlayers();
            DamagePlayers(handlers);
        }
        Destroy(gameObject); //edit if pool
    }

    private PlayerDamageHandler[] DetectAffectedPlayers()
    {
        List<PlayerDamageHandler> handlers = new();
        Vector3 grenadePos = transform.position;
        
        foreach (Player player in OnlinePlayersRegistry.GetAll().Values)
        {
            float dist = Vector3.Distance(player.transform.position, grenadePos);
            bool inHighRange = dist < highDmgRadius;
            bool inMediumRange = dist < mediumDmgRadius;
            bool inLowRange = dist < lowDmgRadius;
            
            if (inHighRange) handlers.Add(new PlayerDamageHandler() { damage = highDmg, player = player });
            else if (inMediumRange) handlers.Add(new PlayerDamageHandler() { damage = mediumDmg, player = player });
            else if (inLowRange) handlers.Add(new PlayerDamageHandler() { damage = lowDmg, player = player });
        }

        return handlers.ToArray();
    }

    private static void DamagePlayers(PlayerDamageHandler[] handlers)
    {
        Player grenadeOwner = Player.OwnerSingleton;
        foreach (PlayerDamageHandler damageHandler in handlers)
        {
            Player target = damageHandler.player;
            float damage = damageHandler.damage;
            
            target.Health.TakeDamage(damage);
            grenadeOwner.RpcManager.SendHealthDamageClientRpc(target.Network.OwnerClientId, damage, Vector3.zero); //todo : double check
        }
    } 
    
}