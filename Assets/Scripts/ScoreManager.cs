using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] private TMP_Text scoreText;
    private int hostScore;
    private int clientScore;

    private void Start()
    {
        UpdateScoreText();
    }
    public override void OnNetworkSpawn()
    {
        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    private void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
            return;

        Egg.OnFellInWater += EggFellInWaterCallback;
        GameManager.OnGameStateChangesd += GameStateChangedCallback;
    }

    public override void OnDestroy()
    {
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        Egg.OnFellInWater -= EggFellInWaterCallback;
        GameManager.OnGameStateChangesd -= GameStateChangedCallback;
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
                ResetScore();
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
    private void UpdateScoreTextClientRpc()
    {
        scoreText.text = "<color=#0055ffff>" + hostScore + "</color> - <color=#ff5500ff>" + clientScore + "</color>";
    }
    private void ResetScore()
    {
        hostScore = 0;
        clientScore = 0;

        UpdateScoreClientRpc(hostScore, clientScore);
        UpdateScoreText();
    }
    private void CheckForEndGame()
    {
        if (hostScore >= 5)
        {
            Hostwin();
        }
        else if (clientScore >= 5)
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
            GameManager.Instance.SetGameState(GameState.Win);
        else
            GameManager.Instance.SetGameState(GameState.Lose);
    }

    [ClientRpc]
    private void ClientWinClientRPC()
    {
        if (IsServer)
            GameManager.Instance.SetGameState(GameState.Lose);
        else
            GameManager.Instance.SetGameState(GameState.Win);
    }
    private void ReUseEgg()
    {
        EggManager.Instance.ReSpawnEgg();
    }
}
