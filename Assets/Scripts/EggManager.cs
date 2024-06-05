using Unity.Netcode;
using UnityEngine;

public class EggManager : NetworkBehaviour
{
    public static EggManager Instance;

    [Header("Elements")]
    [SerializeField] private Egg eggPrefab;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
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
        eggInstance.transform.SetParent(transform);
    }
    public void ReSpawnEgg()
    {
        if (!IsServer)
            return;

        if (transform.childCount <= 0)
            return;

        transform.GetChild(0).GetComponent<Egg>().Reuse();
    }
    public void DeSpawnEgg()
    {
        if (!IsServer)
            return;

        if (transform.childCount <= 0)
            return;

        transform.GetChild(0).GetComponent<Egg>().GetComponent<NetworkObject>().Despawn();
    }
}
