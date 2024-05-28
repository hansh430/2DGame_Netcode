using System;
using Unity.Netcode;

public class PlayerSelector : NetworkBehaviour
{
    private bool isHostTurn=true;
    public override void OnNetworkSpawn()
    {
        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    private void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
            return;
        GameManager.OnGameStateChangesd += GameStateChangedCallback;
        Egg.OnHit += SwitchPlayers;
    }


    public override void OnDestroy()
    {
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        GameManager.OnGameStateChangesd -= GameStateChangedCallback;
        Egg.OnHit -= SwitchPlayers;
    }
    private void GameStateChangedCallback(GameState state)
    {
        switch (state)
        {
            case GameState.Game:
                Initialize();
                break;
        }
    }
   
    private void Initialize()
    {
        PlayerStateManager[] playerStateManagers = FindObjectsOfType<PlayerStateManager>();

        for (int i = 0; i < playerStateManagers.Length; i++)
        {
            if (playerStateManagers[i].GetComponent<NetworkObject>().IsOwnedByServer)
            {
                if (isHostTurn)
                    playerStateManagers[i].Enable();
                else
                    playerStateManagers[i].Disable();
            }
            else
            {
                if (isHostTurn)
                    playerStateManagers[i].Disable();
                else
                    playerStateManagers[i].Enable();
            }
        }
    }
    private void SwitchPlayers()
    {
        isHostTurn = !isHostTurn;
        Initialize();
    }
}
