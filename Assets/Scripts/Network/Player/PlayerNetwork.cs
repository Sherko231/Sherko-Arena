using System;
using Sherko.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text nameTMP;
    private Player _player;

    public NetworkVariable<CharacterType> CharacterType { get; } = new(writePerm: NetworkVariableWritePermission.Owner);
    
    public event Action OnPlayerNetworkSpawn;
    public event Action OnPlayerNetworkDespawn;
    
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public override void OnNetworkSpawn()
    {
        SetName();
        if (IsOwner)
        {
            CharacterType.Value = CharacterSelector.Instance.Selection;
        }
        else
        {
            DisableGravity();
        }
        OnlinePlayersRegistry.Register(OwnerClientId, _player);
        OnlinePlayersRegistry.RegisterInstanceId(_player.gameObject.GetInstanceID(), _player);
        transform.position = SpawnPointsRegistry.Instance.GetRandomSpawnPoint();
        OnPlayerNetworkSpawn?.Invoke();
    }

    public void Init()
    {
        if (IsOwner) ApplyLocalLayer();
    }

    public override void OnNetworkDespawn()
    {
        OnlinePlayersRegistry.Deregister(OwnerClientId);
        OnlinePlayersRegistry.DeregisterInstanceId(_player.GetInstanceID());
        OnPlayerNetworkDespawn?.Invoke();
    }

    public static void PauseAndDisconnect()
    {
        SherkoUtils.ToggleCursor(!SherkoUtils.IsCursorVisible);
        NetworkManager.Singleton.Shutdown();
        JoinUI.Instance.ToggleUI(!JoinUI.Instance.Toggled);
    }

    private void ApplyLocalLayer()
    {
        foreach (Transform trans in GetComponentsInChildren<Transform>(true))
        {
            if (trans.gameObject.layer == LayerMask.NameToLayer("Gun")) continue;
            if (trans.gameObject.layer == LayerMask.NameToLayer("PlayerParticle")) continue;
            trans.gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
        }
    }

    private void DisableGravity()
    {
        if (TryGetComponent(out Rigidbody rb)) rb.useGravity = false;
    }

    private void SetName()
    {
        nameTMP.text = "Player " + (OwnerClientId + 1);
    }

}
