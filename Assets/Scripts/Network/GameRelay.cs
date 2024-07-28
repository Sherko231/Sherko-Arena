using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class GameRelay : MonoBehaviour
{
    public static GameRelay Instance { get; private set; }
    public string JoinCode { get; private set; }

    public event Action OnJoin;
    
    [SerializeField] private TMP_Text tmp;

    private void Awake() => Instance = this;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(8);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            JoinCode = joinCode;
            Debug.Log(joinCode);

            RelayServerData relayServerData = new(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            tmp.text = "Code : " + JoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with : " + joinCode);
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            tmp.text = "Code : " + JoinCode;
            Debug.Log("Joined Relay with : " + joinCode);
            OnJoin?.Invoke();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
