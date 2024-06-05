using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public static Action<GameState> OnGameStateChangesd;

    private GameState gameState;

    [SerializeField] private int connectedPlayers;

    public GameState GameState { get { return gameState; } set { gameState = value; } }
    public int ConnectedPlayer { get { return connectedPlayers; } set { connectedPlayers = value; } }
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

        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisConnectedCallback;
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        Debug.Log("This is called");
        connectedPlayers++;
        if (connectedPlayers >= 2)
            StartGame();
    }
    private void Singleton_OnClientDisConnectedCallback(ulong obj)
    {
        connectedPlayers = 0;
        RemoveEvents();
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

    private void RemoveEvents()
    {
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisConnectedCallback;
    }
    public void SetGameState(GameState state)
    {
        this.gameState = state;
        OnGameStateChangesd?.Invoke(gameState);
    }
}

public enum GameState
{
    Menu, Game, Win, Lose
}
