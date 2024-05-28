using System;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public static Action<GameState> OnGameStateChangesd;

    private GameState gameState;
    private int connectedPlayers;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public override void OnNetworkSpawn()
    {
        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }
    private void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
            return;

        connectedPlayers++;
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        connectedPlayers++;
        if (connectedPlayers >= 2)
            StartGame();
    }

    private void StartGame()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        gameState = GameState.Game;
        OnGameStateChangesd?.Invoke(gameState);
    }

    private void Start()
    {
        gameState = GameState.Menu;
    }
    public override void OnDestroy()
    {
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
    }
}
public enum GameState
{
    Menu, Game, Win, Lose
}