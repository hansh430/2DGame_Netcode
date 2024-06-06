using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    [Header("Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text targetScoreText;

    private int hostScore;
    private int clientScore;
    private int targetScore = 3;

    public int TargetScore { get { return targetScore; } set { targetScore = value; } }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        UpdateScoreText();
    }
    public override void OnNetworkSpawn()
    {
        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisConnectedCallback;
    }

    private void Singleton_OnClientDisConnectedCallback(ulong obj)
    {
        RemoveAllEvents();
    }

    private void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
            return;

        Egg.OnFellInWater += EggFellInWaterCallback;
        GameManager.OnGameStateChangesd += GameStateChangedCallback;
    }

    private void RemoveAllEvents()
    {
        Egg.OnFellInWater -= EggFellInWaterCallback;
        GameManager.OnGameStateChangesd -= GameStateChangedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisConnectedCallback;
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
    }

    private void EggFellInWaterCallback()
    {
        if (PlayerSelector.Instance.IsHostTurn)
            clientScore++;
        else
            hostScore++;

        UpdateScoreClientRpc(hostScore, clientScore);
        UpdateScoreText();
        CheckForEndGame();
    }
    private void GameStateChangedCallback(GameState state)
    {
        switch (state)
        {
            case GameState.Game:
                ResetScore(targetScore);
                break;
        }
    }

    private void UpdateScoreText()
    {
        UpdateScoreTextClientRpc();
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int hostScore, int clientScore)
    {
        this.hostScore = hostScore;
        this.clientScore = clientScore;
    }

    [ClientRpc]
    private void UpdateTargetScoreClientRpc(int targetScore)
    {
        this.targetScore = targetScore;
    }

    [ClientRpc]
    private void UpdateScoreTextClientRpc()
    {
        scoreText.text = "<color=#0055ffff>" + hostScore + "</color> - <color=#ff5500ff>" + clientScore + "</color>";
    }

    [ClientRpc]
    private void UpdateTargetScoreTextClientRpc()
    {
        targetScoreText.text = "" + targetScore;
    }
    public void ResetScore(int updatedScore)
    {
        hostScore = 0;
        clientScore = 0;
        targetScore = updatedScore;
        UpdateScoreClientRpc(hostScore, clientScore);
        UpdateTargetScoreClientRpc(targetScore);
        UpdateTargetScoreTextClientRpc();
        UpdateScoreText();
    }
    private void CheckForEndGame()
    {
        if (hostScore >= targetScore)
        {
            Hostwin();
        }
        else if (clientScore >= targetScore)
        {
            ClientWin();
        }
        else
        {
            ReUseEgg();
        }
    }

    private void ClientWin()
    {
        ClientWinClientRPC();
    }

    private void Hostwin()
    {
        HostWinClientRPC();
    }

    [ClientRpc]
    private void HostWinClientRPC()
    {
        if (IsServer)
        {
            UIManager.Instance.WaitingForNextGame(false, true);
            GameManager.Instance.SetGameState(GameState.Win);
        }
        else
        {
            UIManager.Instance.WaitingForNextGame(true, false);
            GameManager.Instance.SetGameState(GameState.Lose);
        }
    }

    [ClientRpc]
    private void ClientWinClientRPC()
    {
        if (IsServer)
        {
            UIManager.Instance.WaitingForNextGame(false, true);
            GameManager.Instance.SetGameState(GameState.Lose);
        }
        else
        {
            UIManager.Instance.WaitingForNextGame(true, false);
            GameManager.Instance.SetGameState(GameState.Win);
        }
    }
    private void ReUseEgg()
    {
        EggManager.Instance.ReSpawnEgg();
    }

}
