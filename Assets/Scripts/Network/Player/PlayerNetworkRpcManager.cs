using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkRpcManager : NetworkBehaviour
{
    [Rpc(SendTo.NotOwner)]
    public void SendHealthDamageClientRpc(ulong playerClientId, float damage, Vector3 bulletVec)
    {
        Player player = OnlinePlayersRegistry.Get(playerClientId);
        if (!player.IsInitialized) return;
        player.Health.TakeDamage(damage, bulletVec);
    }
    
    [Rpc(SendTo.NotOwner)]
    public void SendDeathConfirmClientRpc(ulong playerId)
    {
        Player player = OnlinePlayersRegistry.Get(playerId);
        if (!player.IsInitialized) return;
        if (player.Health.IsAlive) player.Health.Die();
    }

    [Rpc(SendTo.NotOwner)]
    public void SendReviveClientRpc(ulong playerId)
    {
        Player player = OnlinePlayersRegistry.Get(playerId);
        if (!player.IsInitialized) return;
        player.Health.Fill();
    }

    [Rpc(SendTo.NotOwner)]
    public void SendPlayerShootClientRpc(ulong playerId)
    {
        Player player = OnlinePlayersRegistry.Get(playerId);
        if (!player.IsInitialized) return;
        player.Gun.Shoot();
    }

    [Rpc(SendTo.NotOwner)]
    public void SendPlayerReloadClientRpc(ulong playerId)
    {
        Player player = OnlinePlayersRegistry.Get(playerId);
        if (!player.IsInitialized) return;
        player.Gun.Reload();
    }

    [Rpc(SendTo.NotOwner)]
    public void SendAbilityClientRpc(ulong playerId, int index)
    {
        Player player = OnlinePlayersRegistry.Get(playerId);
        if (!player.IsInitialized) return;
        player.PlayerAbilities.ForceExecuteAbility(index);
    }
    
    [Rpc(SendTo.NotOwner)]
    public void SendWallRunVfxClientRpc(ulong playerId, bool triggered)
    {
        Player player = OnlinePlayersRegistry.Get(playerId);
        if (!player.IsInitialized) return;
        player.FX.ToggleWallRunVFX(triggered);
    }
    
    [Rpc(SendTo.NotOwner)]
    public void SendStickClientRpc(ulong playerId, float duration)
    {
        Player player = OnlinePlayersRegistry.Get(playerId);
        if (!player.IsInitialized) return;
        player.Controller.ControlsDisabled = true;
        player.StartCoroutine(StartStickTimer(player, duration));
    }

    [Rpc(SendTo.NotOwner)]
    public void SendPauseAndDisconnectClientRpc()
    {
        PlayerNetwork.PauseAndDisconnect();
    }

    private static IEnumerator StartStickTimer(Player player, float duration)
    {
        player.FX.ToggleWebbedMesh(true);
        yield return new WaitForSeconds(duration);
        player.Controller.ControlsDisabled = false;
        player.FX.ToggleWebbedMesh(false);
    }
}
