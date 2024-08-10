using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class WebProjectile : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0), SuffixLabel("sec")] 
    private float stickDuration = 1.0f;
    
    [SerializeField, MinValue(0), SuffixLabel("sec")] 
    private float lifeTime = 2f;
    
    [SerializeField] private LayerMask targetLayers;
    
    public ulong ThrowerClientId { get; set; }

    private bool _IsLocalGrenade => OnlinePlayersRegistry.Get(ThrowerClientId).Network.IsOwner;
    
    private Rigidbody _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        StartCoroutine(StartSelfDestructTimer());
    }

    private void OnTriggerEnter(Collider other)
    {
        LayerMask otherLayer = other.gameObject.layer;
        
        if (IsTargetLayer(otherLayer))
        {
            int instanceId = other.gameObject.GetInstanceID(); 
            Player player = OnlinePlayersRegistry.GetByInstanceId(instanceId);
            if (_IsLocalGrenade) Affect(player);
        }
        
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Launch(Vector3 dir, float shootPower)
    {
        _rb.AddForce(dir * shootPower, ForceMode.Impulse);
    }

    private void Affect(Player target)
    {
        Player sender = OnlinePlayersRegistry.Get(ThrowerClientId);
        sender.RpcManager.SendStickClientRpc(target.RpcManager.OwnerClientId, stickDuration);
        sender.StartCoroutine(StartUnToggleWebTimer(target));
    }

    private IEnumerator StartUnToggleWebTimer(Player target)
    {
        target.FX.ToggleWebbedMesh(true);
        yield return new WaitForSeconds(stickDuration);
        target.FX.ToggleWebbedMesh(false);
    }
    
    private IEnumerator StartSelfDestructTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    
    private bool IsTargetLayer(int layer)
    {
        return (targetLayers.value & (1 << layer)) != 0;
    }
}
