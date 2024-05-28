using Unity.Netcode;
using UnityEngine;

public class EggManager : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] private Egg eggPrefab;

    private void Start()
    {
        GameManager.OnGameStateChangesd += GameStateChangedCallback;
    }
    public override void OnDestroy()
    {
        GameManager.OnGameStateChangesd -= GameStateChangedCallback;
    }
    private void GameStateChangedCallback(GameState state)
    {
        switch (state)
        {
            case GameState.Game:
                SpawnEgg();
                break;
        }
    }

    private void SpawnEgg()
    {
        if (!IsServer)
            return;

        Egg eggInstance = Instantiate(eggPrefab, Vector2.up * 5, Quaternion.identity);
        eggInstance.GetComponent<NetworkObject>().Spawn();
    }
}
