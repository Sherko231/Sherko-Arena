using Unity.Netcode;

public class PlayerNetworkRpcManager : NetworkBehaviour
{
    [Rpc(SendTo.NotOwner)]
    public void SendHealthDamageClientRpc(ulong playerClientId, float damage)
    {
        Player player = OnlinePlayersRegistry.Get(playerClientId);
        if (!player.IsInitialized) return;
        player.Health.TakeDamage(damage);
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
}
