using System;
using System.Collections;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    private const CharacterType k_targetCharacter = CharacterType.Blastback;
    
    [SerializeField] private float launchForce = 25f;
    [SerializeField] private float lifetime = 3f;

    private bool _launched;

    private void Awake()
    {
        StartCoroutine(SelfDestruct());
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerLaunchPad(other.gameObject.GetInstanceID());
        _launched = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_launched) TriggerLaunchPad(other.gameObject.GetInstanceID());
        _launched = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void TriggerLaunchPad(int instanceId)
    {
        Player player = OnlinePlayersRegistry.GetByInstanceId(instanceId);
        
        if (!IsTargetCharacter(player)) return;
        
        Launch(player);
        Destroy(gameObject);
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void Launch(Player player)
    {
        Vector3 playerVelo = player.Rb.velocity;
        player.Rb.velocity = new(playerVelo.x, 0, playerVelo.z);
        player.Rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);
    }

    private static bool IsTargetCharacter(Player player)
    {
        return player.Network.CharacterType.Value == k_targetCharacter;
    }
}
