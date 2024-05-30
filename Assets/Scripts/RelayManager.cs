using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : NetworkBehaviour
{
    public static RelayManager Instance;

    const int maxConnection = 1;
    public string RelayJoinCode;
    [Header("Elements")]
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInputField;

    public Action WaitingErrorAction;
    public Action WaitingSuccessAction;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        AuthenticatingPlayer();
    }

    private async void AuthenticatingPlayer()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerID = AuthenticationService.Instance.PlayerId;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task<RelayServerData> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null)
    {
        Allocation allocation;
        string createJoinCode;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay create allocation request failed {e.Message}");
            throw;
        }
        Debug.Log($"server:{allocation.ConnectionData[0]}{allocation.ConnectionData[1]}");
        Debug.Log($"server:{allocation.AllocationId}");

        try
        {
            createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeText.text = createJoinCode;
        }
        catch
        {
            Debug.LogError($"Relay create join code request failed");
            throw;
        }
        return new RelayServerData(allocation, "dtls");
    }

    public IEnumerator ConfigureTransportAndStartNgoAsHost()
    {
        var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(maxConnection);
        while (!serverRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
        if (serverRelayUtilityTask.IsFaulted)
        {
            Debug.LogError("Exception thrown when attempting to start Relay server. Exception: " + serverRelayUtilityTask.Exception.Message);
            yield break;
        }
        var relayServerData = serverRelayUtilityTask.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        Debug.Log("Coonection Data: " + relayServerData.ConnectionData);
        NetworkManager.Singleton.StartHost();
        yield return null;
    }

    public async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }
        Debug.Log($"client:{allocation.ConnectionData[0]}{allocation.ConnectionData[1]}");
        Debug.Log($"host:{allocation.HostConnectionData[0]}{allocation.HostConnectionData[1]}");
        Debug.Log($"client:{allocation.AllocationId}");
        return new RelayServerData(allocation, "dtls");
    }
    public IEnumerator ConfigureTransportAndStartNgoAsConnectingPlayer()
    {
        var clientRelayUtilityTask = JoinRelayServerFromJoinCode(joinCodeInputField.text);
        while (!clientRelayUtilityTask.IsCompleted)
        {
            WaitingSuccessAction?.Invoke();
            yield return null;
        }
        if (clientRelayUtilityTask.IsFaulted)
        {
            Debug.LogError("Exception thrown when attempting to connet to Relay server. Exception: " + clientRelayUtilityTask.Exception.Message);
            WaitingErrorAction?.Invoke();
            yield break;
        }

        var relayServerData = clientRelayUtilityTask.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();
        yield return null;
    }
}
